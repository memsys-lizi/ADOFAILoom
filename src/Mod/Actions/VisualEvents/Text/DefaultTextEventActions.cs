using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class DefaultTextEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public DefaultTextEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            DefaultTextEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            DefaultTextEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.SetDefaultText,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.SetDefaultText, token);

        private static LevelEvent Create(scnEditor editor, DefaultTextEventCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.SetDefaultText);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            SetTween(e, input.AngleOffset, input.Duration, input.Ease, input.EventTag);
            int count = 0;
            count += SetColor(e, "defaultTextColor", input.DefaultTextColor);
            count += SetColor(e, "defaultTextShadowColor", input.DefaultTextShadowColor);
            count += SetVector(e, "levelTitlePosition", input.LevelTitlePosition);
            count += SetString(e, "levelTitleText", input.LevelTitleText);
            count += SetString(e, "congratsText", input.CongratsText);
            count += SetString(e, "perfectText", input.PerfectText);
            if (count == 0)
                throw new ArgumentException(
                    "SetDefaultText must enable at least one text property."
                );
            return e;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent e,
            DefaultTextEventUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, input.Floor.Value);
                e.floor = input.Floor.Value;
            }
            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset.Value);
            if (input.Duration.HasValue)
                VisualEventValidator.SetFloat(e, "duration", input.Duration.Value);
            if (input.Ease.HasValue)
                VisualEventValidator.SetEnum(e, "ease", input.Ease.Value);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
            var disabled = new DisabledPropertySet<DefaultTextProperty>(input.DisabledProperties);
            disabled.Apply(
                e,
                DefaultTextProperty.DefaultTextColor,
                input.DefaultTextColor != null,
                "defaultTextColor"
            );
            disabled.Apply(
                e,
                DefaultTextProperty.DefaultTextShadowColor,
                input.DefaultTextShadowColor != null,
                "defaultTextShadowColor"
            );
            disabled.Apply(
                e,
                DefaultTextProperty.LevelTitlePosition,
                input.LevelTitlePosition != null,
                "levelTitlePosition"
            );
            disabled.Apply(
                e,
                DefaultTextProperty.LevelTitleText,
                input.LevelTitleText != null,
                "levelTitleText"
            );
            disabled.Apply(
                e,
                DefaultTextProperty.CongratsText,
                input.CongratsText != null,
                "congratsText"
            );
            disabled.Apply(
                e,
                DefaultTextProperty.PerfectText,
                input.PerfectText != null,
                "perfectText"
            );
            if (input.DefaultTextColor != null)
                VisualEventValidator.SetColor(e, "defaultTextColor", input.DefaultTextColor);
            if (input.DefaultTextShadowColor != null)
                VisualEventValidator.SetColor(
                    e,
                    "defaultTextShadowColor",
                    input.DefaultTextShadowColor
                );
            if (input.LevelTitlePosition != null)
                VisualEventValidator.SetVector2(e, "levelTitlePosition", input.LevelTitlePosition);
            if (input.LevelTitleText != null)
                VisualEventValidator.SetString(e, "levelTitleText", input.LevelTitleText);
            if (input.CongratsText != null)
                VisualEventValidator.SetString(e, "congratsText", input.CongratsText);
            if (input.PerfectText != null)
                VisualEventValidator.SetString(e, "perfectText", input.PerfectText);
            return e;
        }

        private static void SetTween(
            LevelEvent e,
            float angle,
            float duration,
            VisualEase ease,
            string? tag
        )
        {
            VisualEventValidator.SetFloat(e, "angleOffset", angle);
            VisualEventValidator.SetFloat(e, "duration", duration);
            VisualEventValidator.SetEnum(e, "ease", ease);
            if (tag != null)
                VisualEventValidator.SetString(e, "eventTag", tag);
        }

        private static int SetColor(LevelEvent e, string key, string? value)
        {
            if (value == null)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetColor(e, key, value);
            return 1;
        }

        private static int SetString(LevelEvent e, string key, string? value)
        {
            if (value == null)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetString(e, key, value);
            return 1;
        }

        private static int SetVector(LevelEvent e, string key, VisualVector2? value)
        {
            if (value == null)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetVector2(e, key, value);
            return 1;
        }
    }
}
