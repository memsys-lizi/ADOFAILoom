using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class VisualEventMutationEngine
    {
        private const int MaximumBatchSize = 256;

        private readonly MainThreadDispatcher dispatcher;

        public VisualEventMutationEngine(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<VisualEventMutationResult> AddAsync<TInput>(
            string expectedRevision,
            TInput[] inputs,
            Func<scnEditor, TInput, LevelEvent> create,
            CancellationToken cancellationToken,
            Action<scnEditor>? afterRefresh = null
        )
        {
            return dispatcher.InvokeAsync(
                () => Add(expectedRevision, inputs, create, afterRefresh),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        public Task<VisualEventMutationResult> UpdateAsync<TInput>(
            string expectedRevision,
            TInput[] inputs,
            LevelEventType requiredType,
            Func<TInput, VisualEventReference> getReference,
            Func<scnEditor, LevelEvent, TInput, LevelEvent> apply,
            CancellationToken cancellationToken,
            Action<scnEditor>? afterRefresh = null
        )
            where TInput : class
        {
            return dispatcher.InvokeAsync(
                () =>
                    Update(
                        expectedRevision,
                        inputs,
                        requiredType,
                        getReference,
                        apply,
                        afterRefresh
                    ),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        public Task<VisualEventMutationResult> RemoveAsync(
            string expectedRevision,
            VisualEventReference[] references,
            LevelEventType requiredType,
            CancellationToken cancellationToken,
            Action<scnEditor>? afterRefresh = null
        )
        {
            return dispatcher.InvokeAsync(
                () => Remove(expectedRevision, references, requiredType, afterRefresh),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        private static VisualEventMutationResult Add<TInput>(
            string expectedRevision,
            TInput[] inputs,
            Func<scnEditor, TInput, LevelEvent> create,
            Action<scnEditor>? afterRefresh
        )
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, expectedRevision);
            ValidateBatch(inputs, "events");

            LevelEvent[] staged = inputs
                .Select(input => create(editor, RequireItem(input, "events")))
                .ToArray();
            int firstIndex = editor.events.Count;
            using (new SaveStateScope(editor))
            {
                foreach (LevelEvent levelEvent in staged)
                {
                    editor.events.Add(levelEvent);
                }

                RefreshEditor(editor, staged.Select(levelEvent => levelEvent.floor));
                afterRefresh?.Invoke(editor);
            }

            VisualEventReference[] references = staged
                .Select(
                    (levelEvent, index) =>
                        VisualEventReader.CreateReference(
                            VisualEventCollection.LevelEvents,
                            firstIndex + index,
                            levelEvent
                        )
                )
                .ToArray();
            return Result(editor, staged.Length, references);
        }

        private static VisualEventMutationResult Update<TInput>(
            string expectedRevision,
            TInput[] inputs,
            LevelEventType requiredType,
            Func<TInput, VisualEventReference> getReference,
            Func<scnEditor, LevelEvent, TInput, LevelEvent> apply,
            Action<scnEditor>? afterRefresh
        )
            where TInput : class
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, expectedRevision);
            ValidateBatch(inputs, "events");

            var usedIndices = new HashSet<int>();
            var staged = new List<StagedUpdate>(inputs.Length);
            foreach (TInput input in inputs)
            {
                TInput requiredInput = RequireItem(input, "events");
                VisualEventReference reference =
                    getReference(requiredInput)
                    ?? throw new ArgumentException("Event reference cannot be null.", "events");
                LevelEvent current = VisualEventReader.ResolveReference(
                    editor,
                    reference,
                    requiredType
                );
                if (!usedIndices.Add(reference.Index))
                {
                    throw new ArgumentException(
                        $"{requiredType} event index {reference.Index} was specified more than once.",
                        "events"
                    );
                }

                LevelEvent updated = apply(editor, current.Copy(), requiredInput);
                if (
                    string.Equals(
                        CanonicalJsonHash.ComputeEventFingerprint(current),
                        CanonicalJsonHash.ComputeEventFingerprint(updated),
                        StringComparison.Ordinal
                    )
                )
                {
                    throw new ArgumentException(
                        $"The update for {requiredType} event index {reference.Index} "
                            + "does not change the event.",
                        "events"
                    );
                }

                staged.Add(new StagedUpdate(reference.Index, current.floor, updated));
            }

            using (new SaveStateScope(editor))
            {
                foreach (StagedUpdate update in staged)
                {
                    editor.events[update.Index] = update.Event;
                }

                RefreshEditor(
                    editor,
                    staged.SelectMany(update => new[] { update.OldFloor, update.Event.floor })
                );
                afterRefresh?.Invoke(editor);
            }

            VisualEventReference[] references = staged
                .Select(update =>
                    VisualEventReader.CreateReference(
                        VisualEventCollection.LevelEvents,
                        update.Index,
                        update.Event
                    )
                )
                .ToArray();
            return Result(editor, staged.Count, references);
        }

        private static VisualEventMutationResult Remove(
            string expectedRevision,
            VisualEventReference[] references,
            LevelEventType requiredType,
            Action<scnEditor>? afterRefresh
        )
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, expectedRevision);
            ValidateBatch(references, "eventRefs");

            var usedIndices = new HashSet<int>();
            var removals = new List<StagedRemoval>(references.Length);
            foreach (VisualEventReference reference in references)
            {
                VisualEventReference requiredReference = RequireItem(reference, "eventRefs");
                LevelEvent current = VisualEventReader.ResolveReference(
                    editor,
                    requiredReference,
                    requiredType
                );
                if (!usedIndices.Add(requiredReference.Index))
                {
                    throw new ArgumentException(
                        $"{requiredType} event index {requiredReference.Index} "
                            + "was specified more than once.",
                        "eventRefs"
                    );
                }

                removals.Add(new StagedRemoval(requiredReference.Index, current.floor));
            }

            using (new SaveStateScope(editor))
            {
                foreach (StagedRemoval removal in removals.OrderByDescending(item => item.Index))
                {
                    editor.events.RemoveAt(removal.Index);
                }

                RefreshEditor(editor, removals.Select(removal => removal.Floor));
                afterRefresh?.Invoke(editor);
            }

            return Result(editor, removals.Count, Array.Empty<VisualEventReference>());
        }

        private static void RefreshEditor(scnEditor editor, IEnumerable<int> affectedFloors)
        {
            editor.ApplyEventsToFloors();
            foreach (int floor in affectedFloors.Distinct().OrderBy(value => value))
            {
                if (floor >= 0 && floor < editor.floors.Count)
                {
                    editor.ShowEventIndicators(editor.floors[floor]);
                }
            }

            if (editor.SelectionIsSingle())
            {
                editor.DecideInspectorTabsAtSelected();
            }
        }

        private static VisualEventMutationResult Result(
            scnEditor editor,
            int affectedCount,
            IReadOnlyList<VisualEventReference> events
        )
        {
            return new VisualEventMutationResult(
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData),
                affectedCount,
                events
            );
        }

        private static void ValidateBatch<T>(T[] values, string parameterName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (values.Length == 0)
            {
                throw new ArgumentException("Batch cannot be empty.", parameterName);
            }

            if (values.Length > MaximumBatchSize)
            {
                throw new ArgumentException(
                    $"Batch cannot contain more than {MaximumBatchSize} items.",
                    parameterName
                );
            }
        }

        private static T RequireItem<T>(T item, string parameterName)
        {
            if (item is null)
            {
                throw new ArgumentException("Batch items cannot be null.", parameterName);
            }

            return item;
        }

        private sealed class StagedUpdate
        {
            public StagedUpdate(int index, int oldFloor, LevelEvent levelEvent)
            {
                Index = index;
                OldFloor = oldFloor;
                Event = levelEvent;
            }

            public int Index { get; }

            public int OldFloor { get; }

            public LevelEvent Event { get; }
        }

        private sealed class StagedRemoval
        {
            public StagedRemoval(int index, int floor)
            {
                Index = index;
                Floor = floor;
            }

            public int Index { get; }

            public int Floor { get; }
        }
    }
}
