using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class FilterEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public FilterEventActions(VisualEventMutationEngine engine)
        {
            this.engine = engine;
        }

        public Task<VisualEventMutationResult> AddAsync(
            string expectedRevision,
            FilterEventCreate[] events,
            CancellationToken cancellationToken)
        {
            return engine.AddAsync(expectedRevision, events, Create, cancellationToken);
        }

        public Task<VisualEventMutationResult> UpdateAsync(
            string expectedRevision,
            FilterEventUpdate[] events,
            CancellationToken cancellationToken)
        {
            return engine.UpdateAsync(
                expectedRevision,
                events,
                LevelEventType.SetFilter,
                input => input.Reference,
                ApplyUpdate,
                cancellationToken);
        }

        public Task<VisualEventMutationResult> RemoveAsync(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken)
        {
            return engine.RemoveAsync(
                expectedRevision,
                eventRefs,
                LevelEventType.SetFilter,
                cancellationToken);
        }

        private static LevelEvent Create(scnEditor editor, FilterEventCreate input)
        {
            var levelEvent = new LevelEvent(input.Floor, LevelEventType.SetFilter);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset);
            VisualEventValidator.SetEnum(levelEvent, "filter", input.Filter);
            VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled);
            VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity);
            VisualEventValidator.SetBool(levelEvent, "disableOthers", input.DisableOthers);
            VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration);
            VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease);
            return levelEvent;
        }

        private static LevelEvent ApplyUpdate(
            scnEditor editor,
            LevelEvent levelEvent,
            FilterEventUpdate input)
        {
            bool changed = false;
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor.Value);
                levelEvent.floor = input.Floor.Value;
                changed = true;
            }

            if (input.AngleOffset.HasValue)
            {
                VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset.Value);
                changed = true;
            }

            if (input.Filter.HasValue)
            {
                VisualEventValidator.SetEnum(levelEvent, "filter", input.Filter.Value);
                changed = true;
            }

            if (input.Enabled.HasValue)
            {
                VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled.Value);
                changed = true;
            }

            if (input.Intensity.HasValue)
            {
                VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity.Value);
                changed = true;
            }

            if (input.DisableOthers.HasValue)
            {
                VisualEventValidator.SetBool(levelEvent, "disableOthers", input.DisableOthers.Value);
                changed = true;
            }

            if (input.Duration.HasValue)
            {
                VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration.Value);
                changed = true;
            }

            if (input.Ease.HasValue)
            {
                VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease.Value);
                changed = true;
            }

            if (!changed)
            {
                throw new ArgumentException("Filter event update contains no changes.");
            }

            return levelEvent;
        }
    }
}
