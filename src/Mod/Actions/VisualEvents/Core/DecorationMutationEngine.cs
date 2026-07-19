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
    internal sealed class DecorationMutationEngine
    {
        private const int MaximumBatchSize = 256;
        private readonly MainThreadDispatcher dispatcher;

        public DecorationMutationEngine(MainThreadDispatcher dispatcher) =>
            this.dispatcher = dispatcher;

        public Task<VisualEventMutationResult> AddAsync<T>(
            string revision,
            T[] inputs,
            Func<scnEditor, T, LevelEvent> create,
            CancellationToken token
        ) =>
            dispatcher.InvokeAsync(
                () => Add(revision, inputs, create),
                McpProtocol.MainThreadTimeout,
                token
            );

        public Task<VisualEventMutationResult> UpdateAsync<T>(
            string revision,
            T[] inputs,
            LevelEventType type,
            Func<T, VisualEventReference> reference,
            Func<scnEditor, LevelEvent, T, LevelEvent> apply,
            CancellationToken token
        )
            where T : class =>
            dispatcher.InvokeAsync(
                () => Update(revision, inputs, type, reference, apply),
                McpProtocol.MainThreadTimeout,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            LevelEventType type,
            CancellationToken token
        ) =>
            dispatcher.InvokeAsync(
                () => Remove(revision, references, type),
                McpProtocol.MainThreadTimeout,
                token
            );

        private static VisualEventMutationResult Add<T>(
            string revision,
            T[] inputs,
            Func<scnEditor, T, LevelEvent> create
        )
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, revision);
            Validate(inputs);
            LevelEvent[] staged = inputs.Select(input => create(editor, Require(input))).ToArray();
            int first = editor.decorations.Count;
            using (new SaveStateScope(editor))
            {
                foreach (LevelEvent item in staged)
                    editor.decorations.Add(item);
                editor.UpdateDecorationObjects();
            }
            return Result(
                editor,
                staged.Length,
                staged
                    .Select(
                        (item, index) =>
                            VisualEventReader.CreateReference(
                                VisualEventCollection.Decorations,
                                first + index,
                                item
                            )
                    )
                    .ToArray()
            );
        }

        private static VisualEventMutationResult Update<T>(
            string revision,
            T[] inputs,
            LevelEventType type,
            Func<T, VisualEventReference> getReference,
            Func<scnEditor, LevelEvent, T, LevelEvent> apply
        )
            where T : class
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, revision);
            Validate(inputs);
            var used = new HashSet<int>();
            var staged = new List<(int Index, LevelEvent Event)>();
            foreach (T input in inputs)
            {
                T value = Require(input);
                VisualEventReference reference =
                    getReference(value)
                    ?? throw new ArgumentException("Event reference cannot be null.");
                LevelEvent current = VisualEventReader.ResolveReference(
                    editor,
                    reference,
                    type,
                    VisualEventCollection.Decorations
                );
                if (!used.Add(reference.Index))
                    throw new ArgumentException(
                        $"Decoration index {reference.Index} was specified more than once."
                    );
                LevelEvent updated = apply(editor, current.Copy(), value);
                if (
                    CanonicalJsonHash.ComputeEventFingerprint(current)
                    == CanonicalJsonHash.ComputeEventFingerprint(updated)
                )
                    throw new ArgumentException(
                        $"The update for decoration index {reference.Index} does not change the event."
                    );
                staged.Add((reference.Index, updated));
            }
            using (new SaveStateScope(editor))
            {
                foreach (var item in staged)
                    editor.decorations[item.Index] = item.Event;
                editor.UpdateDecorationObjects();
            }
            return Result(
                editor,
                staged.Count,
                staged
                    .Select(item =>
                        VisualEventReader.CreateReference(
                            VisualEventCollection.Decorations,
                            item.Index,
                            item.Event
                        )
                    )
                    .ToArray()
            );
        }

        private static VisualEventMutationResult Remove(
            string revision,
            VisualEventReference[] references,
            LevelEventType type
        )
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, revision);
            Validate(references);
            var indices = new HashSet<int>();
            foreach (VisualEventReference reference in references)
            {
                VisualEventReference value = Require(reference);
                VisualEventReader.ResolveReference(
                    editor,
                    value,
                    type,
                    VisualEventCollection.Decorations
                );
                if (!indices.Add(value.Index))
                    throw new ArgumentException(
                        $"Decoration index {value.Index} was specified more than once."
                    );
            }
            using (new SaveStateScope(editor))
            {
                foreach (int index in indices.OrderByDescending(value => value))
                    editor.decorations.RemoveAt(index);
                editor.UpdateDecorationObjects();
            }
            return Result(editor, indices.Count, Array.Empty<VisualEventReference>());
        }

        private static VisualEventMutationResult Result(
            scnEditor editor,
            int count,
            IReadOnlyList<VisualEventReference> events
        ) =>
            new VisualEventMutationResult(
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData),
                count,
                events
            );

        private static void Validate<T>(T[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length == 0 || values.Length > MaximumBatchSize)
                throw new ArgumentException(
                    $"Batch size must be between 1 and {MaximumBatchSize}."
                );
        }

        private static T Require<T>(T value)
        {
            if (value is null)
                throw new ArgumentException("Batch items cannot be null.");
            return value;
        }
    }
}
