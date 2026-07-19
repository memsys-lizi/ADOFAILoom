using System.Collections.Generic;
using System.Text.Json.Serialization;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualEase
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        Flash,
        InFlash,
        OutFlash,
        InOutFlash,
    }

    internal sealed class VisualVector2
    {
        [JsonRequired]
        public float X { get; set; }

        [JsonRequired]
        public float Y { get; set; }
    }

    internal sealed class VisualEventMutationResult
    {
        public VisualEventMutationResult(
            string revision,
            int affectedCount,
            IReadOnlyList<VisualEventReference> events
        )
        {
            Revision = revision;
            AffectedCount = affectedCount;
            Events = events;
        }

        public string Revision { get; }

        public int AffectedCount { get; }

        public IReadOnlyList<VisualEventReference> Events { get; }
    }
}
