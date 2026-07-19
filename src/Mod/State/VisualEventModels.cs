using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADOFAILoom.State
{
    internal enum VisualEventCollection
    {
        LevelEvents,
        Decorations,
    }

    internal sealed class VisualEventReference
    {
        [JsonRequired]
        [Description("Exact source collection. Mutating tools currently accept LevelEvents only.")]
        public VisualEventCollection Collection { get; set; }

        [JsonRequired]
        [Description("Current zero-based index inside the source collection.")]
        public int Index { get; set; }

        [JsonRequired]
        [Description("Exact case-sensitive game event type.")]
        public string EventType { get; set; } = string.Empty;

        [JsonRequired]
        [Description("Lowercase SHA-256 fingerprint returned by list_visual_events.")]
        public string Fingerprint { get; set; } = string.Empty;
    }

    internal sealed class VisualEventSnapshot
    {
        public VisualEventSnapshot(
            VisualEventReference reference,
            int floor,
            string eventType,
            bool active,
            bool visible,
            bool locked,
            JsonElement properties,
            IReadOnlyList<string> disabledProperties
        )
        {
            Reference = reference;
            Floor = floor;
            EventType = eventType;
            Active = active;
            Visible = visible;
            Locked = locked;
            Properties = properties;
            DisabledProperties = disabledProperties;
        }

        public VisualEventReference Reference { get; }

        public int Floor { get; }

        public string EventType { get; }

        public bool Active { get; }

        public bool Visible { get; }

        public bool Locked { get; }

        public JsonElement Properties { get; }

        public IReadOnlyList<string> DisabledProperties { get; }
    }

    internal sealed class VisualEventPage
    {
        public VisualEventPage(
            string revision,
            int total,
            int offset,
            int limit,
            IReadOnlyList<VisualEventSnapshot> events
        )
        {
            Revision = revision;
            Total = total;
            Offset = offset;
            Limit = limit;
            Events = events;
        }

        public string Revision { get; }

        public int Total { get; }

        public int Offset { get; }

        public int Limit { get; }

        public IReadOnlyList<VisualEventSnapshot> Events { get; }
    }
}
