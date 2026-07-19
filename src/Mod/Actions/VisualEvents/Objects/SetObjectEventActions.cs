using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class SetObjectEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public SetObjectEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            SetObjectEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            SetObjectEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.SetObject,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.SetObject, token);

        private static LevelEvent Create(scnEditor editor, SetObjectEventCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.SetObject);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            SetTween(e, input.AngleOffset, input.Duration, input.Ease, input.EventTag);
            SetTag(e, input.Tag);
            int count =
                SetOptionalColor(e, "planetColor", input.PlanetColor)
                + SetOptionalColor(e, "planetTailColor", input.PlanetTailColor)
                + SetOptional(e, "trackAngle", input.TrackAngle)
                + SetOptional(e, "trackColorType", input.TrackColorType)
                + SetOptionalColor(e, "trackColor", input.TrackColor)
                + SetOptionalColor(e, "secondaryTrackColor", input.SecondaryTrackColor)
                + SetOptional(e, "trackColorAnimDuration", input.TrackColorAnimationDuration)
                + SetOptional(e, "trackOpacity", input.TrackOpacity)
                + SetOptional(e, "trackStyle", input.TrackStyle)
                + SetOptional(e, "trackIcon", input.TrackIcon)
                + SetOptional(e, "trackIconAngle", input.TrackIconAngle)
                + SetOptional(e, "trackIconFlipped", input.TrackIconFlipped)
                + SetOptional(e, "trackRedSwirl", input.TrackRedSwirl)
                + SetOptional(e, "trackGraySetSpeedIcon", input.TrackGraySetSpeedIcon)
                + SetOptional(e, "trackGlowEnabled", input.TrackGlowEnabled)
                + SetOptionalColor(e, "trackGlowColor", input.TrackGlowColor)
                + SetOptional(e, "trackIconOutlines", input.TrackIconOutlines);
            if (count == 0)
                throw new ArgumentException("SetObject must enable at least one object property.");
            return e;
        }

        private static LevelEvent Update(scnEditor editor, LevelEvent e, SetObjectEventUpdate input)
        {
            ApplyTween(editor, e, input);
            if (input.Tag != null)
                SetTag(e, input.Tag);
            var disabled = new DisabledPropertySet<SetObjectProperty>(input.DisabledProperties);
            ApplyDisabled(disabled, e, input);
            if (input.PlanetColor != null)
                VisualEventValidator.SetColor(e, "planetColor", input.PlanetColor);
            if (input.PlanetTailColor != null)
                VisualEventValidator.SetColor(e, "planetTailColor", input.PlanetTailColor);
            if (input.TrackAngle.HasValue)
                VisualEventValidator.SetFloat(e, "trackAngle", input.TrackAngle.Value);
            if (input.TrackColorType.HasValue)
                VisualEventValidator.SetEnum(e, "trackColorType", input.TrackColorType.Value);
            if (input.TrackColor != null)
                VisualEventValidator.SetColor(e, "trackColor", input.TrackColor);
            if (input.SecondaryTrackColor != null)
                VisualEventValidator.SetColor(e, "secondaryTrackColor", input.SecondaryTrackColor);
            if (input.TrackColorAnimationDuration.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackColorAnimDuration",
                    input.TrackColorAnimationDuration.Value
                );
            if (input.TrackOpacity.HasValue)
                VisualEventValidator.SetFloat(e, "trackOpacity", input.TrackOpacity.Value);
            if (input.TrackStyle.HasValue)
                VisualEventValidator.SetEnum(e, "trackStyle", input.TrackStyle.Value);
            if (input.TrackIcon.HasValue)
                VisualEventValidator.SetEnum(e, "trackIcon", input.TrackIcon.Value);
            if (input.TrackIconAngle.HasValue)
                VisualEventValidator.SetFloat(e, "trackIconAngle", input.TrackIconAngle.Value);
            if (input.TrackIconFlipped.HasValue)
                VisualEventValidator.SetBool(e, "trackIconFlipped", input.TrackIconFlipped.Value);
            if (input.TrackRedSwirl.HasValue)
                VisualEventValidator.SetBool(e, "trackRedSwirl", input.TrackRedSwirl.Value);
            if (input.TrackGraySetSpeedIcon.HasValue)
                VisualEventValidator.SetBool(
                    e,
                    "trackGraySetSpeedIcon",
                    input.TrackGraySetSpeedIcon.Value
                );
            if (input.TrackGlowEnabled.HasValue)
                VisualEventValidator.SetBool(e, "trackGlowEnabled", input.TrackGlowEnabled.Value);
            if (input.TrackGlowColor != null)
                VisualEventValidator.SetColor(e, "trackGlowColor", input.TrackGlowColor);
            if (input.TrackIconOutlines.HasValue)
                VisualEventValidator.SetBool(e, "trackIconOutlines", input.TrackIconOutlines.Value);
            return e;
        }

        private static void ApplyDisabled(
            DisabledPropertySet<SetObjectProperty> d,
            LevelEvent e,
            SetObjectEventUpdate i
        )
        {
            d.Apply(e, SetObjectProperty.PlanetColor, i.PlanetColor != null, "planetColor");
            d.Apply(
                e,
                SetObjectProperty.PlanetTailColor,
                i.PlanetTailColor != null,
                "planetTailColor"
            );
            d.Apply(e, SetObjectProperty.TrackAngle, i.TrackAngle.HasValue, "trackAngle");
            d.Apply(
                e,
                SetObjectProperty.TrackColorType,
                i.TrackColorType.HasValue,
                "trackColorType"
            );
            d.Apply(e, SetObjectProperty.TrackColor, i.TrackColor != null, "trackColor");
            d.Apply(
                e,
                SetObjectProperty.SecondaryTrackColor,
                i.SecondaryTrackColor != null,
                "secondaryTrackColor"
            );
            d.Apply(
                e,
                SetObjectProperty.TrackColorAnimationDuration,
                i.TrackColorAnimationDuration.HasValue,
                "trackColorAnimDuration"
            );
            d.Apply(e, SetObjectProperty.TrackOpacity, i.TrackOpacity.HasValue, "trackOpacity");
            d.Apply(e, SetObjectProperty.TrackStyle, i.TrackStyle.HasValue, "trackStyle");
            d.Apply(e, SetObjectProperty.TrackIcon, i.TrackIcon.HasValue, "trackIcon");
            d.Apply(
                e,
                SetObjectProperty.TrackIconAngle,
                i.TrackIconAngle.HasValue,
                "trackIconAngle"
            );
            d.Apply(
                e,
                SetObjectProperty.TrackIconFlipped,
                i.TrackIconFlipped.HasValue,
                "trackIconFlipped"
            );
            d.Apply(e, SetObjectProperty.TrackRedSwirl, i.TrackRedSwirl.HasValue, "trackRedSwirl");
            d.Apply(
                e,
                SetObjectProperty.TrackGraySetSpeedIcon,
                i.TrackGraySetSpeedIcon.HasValue,
                "trackGraySetSpeedIcon"
            );
            d.Apply(
                e,
                SetObjectProperty.TrackGlowEnabled,
                i.TrackGlowEnabled.HasValue,
                "trackGlowEnabled"
            );
            d.Apply(
                e,
                SetObjectProperty.TrackGlowColor,
                i.TrackGlowColor != null,
                "trackGlowColor"
            );
            d.Apply(
                e,
                SetObjectProperty.TrackIconOutlines,
                i.TrackIconOutlines.HasValue,
                "trackIconOutlines"
            );
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

        private static void ApplyTween(scnEditor editor, LevelEvent e, SetObjectEventUpdate i)
        {
            if (i.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, i.Floor.Value);
                e.floor = i.Floor.Value;
            }
            if (i.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(e, "angleOffset", i.AngleOffset.Value);
            if (i.Duration.HasValue)
                VisualEventValidator.SetFloat(e, "duration", i.Duration.Value);
            if (i.Ease.HasValue)
                VisualEventValidator.SetEnum(e, "ease", i.Ease.Value);
            if (i.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", i.EventTag);
        }

        private static void SetTag(LevelEvent e, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("SetObject tag cannot be empty or whitespace.");
            VisualEventValidator.SetString(e, "tag", tag);
        }

        private static int SetOptionalColor(LevelEvent e, string key, string? value)
        {
            if (value == null)
                return Disable(e, key);
            VisualEventValidator.SetColor(e, key, value);
            return 1;
        }

        private static int SetOptional(LevelEvent e, string key, float? value)
        {
            if (!value.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetFloat(e, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent e, string key, bool? value)
        {
            if (!value.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetBool(e, key, value.Value);
            return 1;
        }

        private static int SetOptional<T>(LevelEvent e, string key, T? value)
            where T : struct, Enum
        {
            if (!value.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetEnum(e, key, value.Value);
            return 1;
        }

        private static int Disable(LevelEvent e, string key)
        {
            VisualEventValidator.Disable(e, key);
            return 0;
        }
    }
}
