using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Threading;

namespace ADOFAILoom.State
{
    internal sealed class VisualEventReader
    {
        private const int MaximumPageSize = 200;

        private readonly MainThreadDispatcher dispatcher;

        public VisualEventReader(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<VisualEventPage> ListAsync(
            int? floorStart,
            int? floorEnd,
            string[]? eventTypes,
            int offset,
            int limit,
            CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(
                () => ListOnMainThread(floorStart, floorEnd, eventTypes, offset, limit),
                McpProtocol.MainThreadTimeout,
                cancellationToken);
        }

        public static VisualEventReference CreateReference(
            VisualEventCollection collection,
            int index,
            LevelEvent levelEvent)
        {
            return new VisualEventReference
            {
                Collection = collection,
                Index = index,
                EventType = levelEvent.eventType.ToString(),
                Fingerprint = CanonicalJsonHash.ComputeEventFingerprint(levelEvent)
            };
        }

        public static LevelEvent ResolveReference(
            scnEditor editor,
            VisualEventReference reference,
            LevelEventType requiredType)
        {
            if (reference == null)
            {
                throw new ArgumentException("Event reference cannot be null.", nameof(reference));
            }

            if (reference.Collection != VisualEventCollection.LevelEvents)
            {
                throw new InvalidOperationException(
                    $"{requiredType} events must reference the LevelEvents collection.");
            }

            if (reference.Index < 0 || reference.Index >= editor.events.Count)
            {
                throw StaleReference(reference);
            }

            LevelEvent levelEvent = editor.events[reference.Index];
            string requiredTypeName = requiredType.ToString();
            if (levelEvent.eventType != requiredType ||
                !string.Equals(reference.EventType, requiredTypeName, StringComparison.Ordinal) ||
                !string.Equals(
                    reference.Fingerprint,
                    CanonicalJsonHash.ComputeEventFingerprint(levelEvent),
                    StringComparison.Ordinal))
            {
                throw StaleReference(reference);
            }

            if (levelEvent.locked)
            {
                throw new InvalidOperationException(
                    $"The referenced {requiredTypeName} event is locked in the editor.");
            }

            return levelEvent;
        }

        private static VisualEventPage ListOnMainThread(
            int? floorStart,
            int? floorEnd,
            string[]? eventTypes,
            int offset,
            int limit)
        {
            scnEditor editor = EditorSession.RequireReadable();
            ValidatePage(offset, limit);
            ValidateFloorRange(editor, floorStart, floorEnd);

            HashSet<string> availableTypes = GCS.levelEventsInfo.Values
                .Where(IsVisual)
                .Select(info => info.type.ToString())
                .ToHashSet(StringComparer.Ordinal);
            HashSet<string>? requestedTypes = ValidateEventTypes(eventTypes, availableTypes);

            var matches = new List<VisualEventSnapshot>();
            AddMatches(
                matches,
                editor.events,
                VisualEventCollection.LevelEvents,
                floorStart,
                floorEnd,
                requestedTypes);
            AddMatches(
                matches,
                editor.decorations,
                VisualEventCollection.Decorations,
                floorStart,
                floorEnd,
                requestedTypes);

            VisualEventSnapshot[] page = matches
                .Skip(offset)
                .Take(limit)
                .ToArray();
            return new VisualEventPage(
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData),
                matches.Count,
                offset,
                limit,
                page);
        }

        private static void AddMatches(
            ICollection<VisualEventSnapshot> target,
            IList<LevelEvent> source,
            VisualEventCollection collection,
            int? floorStart,
            int? floorEnd,
            ISet<string>? requestedTypes)
        {
            for (int index = 0; index < source.Count; index++)
            {
                LevelEvent levelEvent = source[index];
                if (!IsVisual(levelEvent.info) ||
                    floorStart.HasValue && levelEvent.floor < floorStart.Value ||
                    floorEnd.HasValue && levelEvent.floor > floorEnd.Value)
                {
                    continue;
                }

                string eventType = levelEvent.eventType.ToString();
                if (requestedTypes != null && !requestedTypes.Contains(eventType))
                {
                    continue;
                }

                string[] disabledProperties = levelEvent.disabled
                    .Where(item => item.Value)
                    .Select(item => item.Key)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray();
                target.Add(new VisualEventSnapshot(
                    CreateReference(collection, index, levelEvent),
                    levelEvent.floor,
                    eventType,
                    levelEvent.active,
                    levelEvent.visible,
                    levelEvent.locked,
                    CanonicalJsonHash.GetEventProperties(levelEvent),
                    disabledProperties));
            }
        }

        private static bool IsVisual(LevelEventInfo info)
        {
            return info != null &&
                   (info.isDecoration ||
                    info.categories.Contains(LevelEventCategory.TrackFx) ||
                    info.categories.Contains(LevelEventCategory.DecorationFx) ||
                    info.categories.Contains(LevelEventCategory.VisualFx));
        }

        private static HashSet<string>? ValidateEventTypes(
            IEnumerable<string>? eventTypes,
            ISet<string> availableTypes)
        {
            if (eventTypes == null)
            {
                return null;
            }

            var result = new HashSet<string>(StringComparer.Ordinal);
            foreach (string eventType in eventTypes)
            {
                if (string.IsNullOrEmpty(eventType) || !availableTypes.Contains(eventType))
                {
                    throw new ArgumentException(
                        $"'{eventType}' is not an exact visual LevelEventType name.",
                        nameof(eventTypes));
                }

                if (!result.Add(eventType))
                {
                    throw new ArgumentException(
                        $"Visual event type '{eventType}' was specified more than once.",
                        nameof(eventTypes));
                }
            }

            if (result.Count == 0)
            {
                throw new ArgumentException(
                    "Event type filter cannot be an empty array.",
                    nameof(eventTypes));
            }

            return result;
        }

        private static void ValidateFloorRange(
            scnEditor editor,
            int? floorStart,
            int? floorEnd)
        {
            int lastFloor = editor.floors.Count - 1;
            if (floorStart.HasValue &&
                (floorStart.Value < LevelEvent.NoFloor || floorStart.Value > lastFloor))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(floorStart),
                    $"Floor start must be between {LevelEvent.NoFloor} and {lastFloor}.");
            }

            if (floorEnd.HasValue &&
                (floorEnd.Value < LevelEvent.NoFloor || floorEnd.Value > lastFloor))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(floorEnd),
                    $"Floor end must be between {LevelEvent.NoFloor} and {lastFloor}.");
            }

            if (floorStart.HasValue && floorEnd.HasValue && floorStart.Value > floorEnd.Value)
            {
                throw new ArgumentException("Floor start cannot be greater than floor end.");
            }
        }

        private static void ValidatePage(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
            }

            if (limit < 1 || limit > MaximumPageSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    $"Limit must be between 1 and {MaximumPageSize}.");
            }
        }

        private static InvalidOperationException StaleReference(VisualEventReference reference)
        {
            return new InvalidOperationException(
                $"The event reference at {reference.Collection}[{reference.Index}] is stale. " +
                "Read visual events again before modifying the level.");
        }
    }
}
