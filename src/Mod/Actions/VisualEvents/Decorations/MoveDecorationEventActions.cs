using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class MoveDecorationEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public MoveDecorationEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            MoveDecorationEventCreate[] events,
            CancellationToken cancellationToken
        ) => engine.AddAsync(revision, events, Create, cancellationToken);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            MoveDecorationEventUpdate[] events,
            CancellationToken cancellationToken
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.MoveDecorations,
                input => input.Reference,
                Update,
                cancellationToken
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken cancellationToken
        ) =>
            engine.RemoveAsync(
                revision,
                references,
                LevelEventType.MoveDecorations,
                cancellationToken
            );

        private static LevelEvent Create(scnEditor editor, MoveDecorationEventCreate input)
        {
            var levelEvent = new LevelEvent(input.Floor, LevelEventType.MoveDecorations);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset);
            VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration);
            VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease);
            SetRequiredTag(levelEvent, input.Tag);
            if (input.EventTag != null)
                VisualEventValidator.SetString(levelEvent, "eventTag", input.EventTag);

            int enabledProperties = 0;
            enabledProperties += SetOptional(levelEvent, "visible", input.Visible);
            enabledProperties += SetOptional(levelEvent, "relativeTo", input.RelativeTo);
            enabledProperties += SetOptionalImage(levelEvent, input.Image);
            enabledProperties += SetOptional(levelEvent, "positionOffset", input.PositionOffset);
            enabledProperties += SetOptional(levelEvent, "pivotOffset", input.PivotOffset);
            enabledProperties += SetOptional(levelEvent, "rotationOffset", input.RotationOffset);
            enabledProperties += SetOptional(levelEvent, "scale", input.Scale);
            enabledProperties += SetOptionalColor(levelEvent, input.Color);
            enabledProperties += SetOptional(levelEvent, "opacity", input.Opacity);
            enabledProperties += SetOptional(levelEvent, "depth", input.Depth);
            enabledProperties += SetOptional(levelEvent, "parallax", input.Parallax);
            enabledProperties += SetOptional(levelEvent, "parallaxOffset", input.ParallaxOffset);
            enabledProperties += SetOptional(levelEvent, "maskingType", input.MaskingType);
            enabledProperties += SetOptionalString(
                levelEvent,
                "maskingTarget",
                input.MaskingTarget
            );
            enabledProperties += SetOptional(levelEvent, "useMaskingDepth", input.UseMaskingDepth);
            enabledProperties += SetOptional(
                levelEvent,
                "maskingFrontDepth",
                input.MaskingFrontDepth
            );
            enabledProperties += SetOptional(
                levelEvent,
                "maskingBackDepth",
                input.MaskingBackDepth
            );
            if (enabledProperties == 0)
                throw new ArgumentException(
                    "MoveDecorations must enable at least one target property."
                );

            return levelEvent;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent levelEvent,
            MoveDecorationEventUpdate input
        )
        {
            ApplyBaseUpdate(editor, levelEvent, input);
            var disabled = new DisabledPropertySet<MoveDecorationProperty>(
                input.DisabledProperties
            );
            ApplyDisabledProperties(disabled, levelEvent, input);

            if (input.Tag != null)
                SetRequiredTag(levelEvent, input.Tag);
            if (input.Visible.HasValue)
                VisualEventValidator.SetBool(levelEvent, "visible", input.Visible.Value);
            if (input.RelativeTo.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "relativeTo", input.RelativeTo.Value);
            if (input.Image != null)
                SetImage(levelEvent, input.Image);
            if (input.PositionOffset != null)
                VisualEventValidator.SetVector2(levelEvent, "positionOffset", input.PositionOffset);
            if (input.PivotOffset != null)
                VisualEventValidator.SetVector2(levelEvent, "pivotOffset", input.PivotOffset);
            if (input.RotationOffset.HasValue)
                VisualEventValidator.SetFloat(
                    levelEvent,
                    "rotationOffset",
                    input.RotationOffset.Value
                );
            if (input.Scale != null)
                VisualEventValidator.SetVector2(levelEvent, "scale", input.Scale);
            if (input.Color != null)
                VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            if (input.Opacity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "opacity", input.Opacity.Value);
            if (input.Depth.HasValue)
                VisualEventValidator.SetInt(levelEvent, "depth", input.Depth.Value);
            if (input.Parallax != null)
                VisualEventValidator.SetVector2(levelEvent, "parallax", input.Parallax);
            if (input.ParallaxOffset != null)
                VisualEventValidator.SetVector2(levelEvent, "parallaxOffset", input.ParallaxOffset);
            if (input.MaskingType.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "maskingType", input.MaskingType.Value);
            if (input.MaskingTarget != null)
                VisualEventValidator.SetString(levelEvent, "maskingTarget", input.MaskingTarget);
            if (input.UseMaskingDepth.HasValue)
                VisualEventValidator.SetBool(
                    levelEvent,
                    "useMaskingDepth",
                    input.UseMaskingDepth.Value
                );
            if (input.MaskingFrontDepth.HasValue)
                VisualEventValidator.SetInt(
                    levelEvent,
                    "maskingFrontDepth",
                    input.MaskingFrontDepth.Value
                );
            if (input.MaskingBackDepth.HasValue)
                VisualEventValidator.SetInt(
                    levelEvent,
                    "maskingBackDepth",
                    input.MaskingBackDepth.Value
                );
            return levelEvent;
        }

        private static void ApplyBaseUpdate(
            scnEditor editor,
            LevelEvent levelEvent,
            MoveDecorationEventUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor.Value);
                levelEvent.floor = input.Floor.Value;
            }

            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset.Value);
            if (input.Duration.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration.Value);
            if (input.Ease.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease.Value);
            if (input.EventTag != null)
                VisualEventValidator.SetString(levelEvent, "eventTag", input.EventTag);
        }

        private static void ApplyDisabledProperties(
            DisabledPropertySet<MoveDecorationProperty> disabled,
            LevelEvent levelEvent,
            MoveDecorationEventUpdate input
        )
        {
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.Visible,
                input.Visible.HasValue,
                "visible"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.RelativeTo,
                input.RelativeTo.HasValue,
                "relativeTo"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.Image,
                input.Image != null,
                "decorationImage"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.PositionOffset,
                input.PositionOffset != null,
                "positionOffset"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.PivotOffset,
                input.PivotOffset != null,
                "pivotOffset"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.RotationOffset,
                input.RotationOffset.HasValue,
                "rotationOffset"
            );
            disabled.Apply(levelEvent, MoveDecorationProperty.Scale, input.Scale != null, "scale");
            disabled.Apply(levelEvent, MoveDecorationProperty.Color, input.Color != null, "color");
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.Opacity,
                input.Opacity.HasValue,
                "opacity"
            );
            disabled.Apply(levelEvent, MoveDecorationProperty.Depth, input.Depth.HasValue, "depth");
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.Parallax,
                input.Parallax != null,
                "parallax"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.ParallaxOffset,
                input.ParallaxOffset != null,
                "parallaxOffset"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.MaskingType,
                input.MaskingType.HasValue,
                "maskingType"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.MaskingTarget,
                input.MaskingTarget != null,
                "maskingTarget"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.UseMaskingDepth,
                input.UseMaskingDepth.HasValue,
                "useMaskingDepth"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.MaskingFrontDepth,
                input.MaskingFrontDepth.HasValue,
                "maskingFrontDepth"
            );
            disabled.Apply(
                levelEvent,
                MoveDecorationProperty.MaskingBackDepth,
                input.MaskingBackDepth.HasValue,
                "maskingBackDepth"
            );
        }

        private static void SetRequiredTag(LevelEvent levelEvent, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("MoveDecorations tag cannot be empty or whitespace.");
            VisualEventValidator.SetString(levelEvent, "tag", tag);
        }

        private static void SetImage(LevelEvent levelEvent, string image)
        {
            EditorAssetValidator.ValidateExistingLevelAsset(image);
            VisualEventValidator.SetFile(levelEvent, "decorationImage", image);
        }

        private static int SetOptionalImage(LevelEvent levelEvent, string? value)
        {
            if (value == null)
                return Disable(levelEvent, "decorationImage");
            SetImage(levelEvent, value);
            return 1;
        }

        private static int SetOptionalColor(LevelEvent levelEvent, string? value)
        {
            if (value == null)
                return Disable(levelEvent, "color");
            VisualEventValidator.SetColor(levelEvent, "color", value);
            return 1;
        }

        private static int SetOptionalString(LevelEvent levelEvent, string key, string? value)
        {
            if (value == null)
                return Disable(levelEvent, key);
            VisualEventValidator.SetString(levelEvent, key, value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, bool? value)
        {
            if (!value.HasValue)
                return Disable(levelEvent, key);
            VisualEventValidator.SetBool(levelEvent, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, int? value)
        {
            if (!value.HasValue)
                return Disable(levelEvent, key);
            VisualEventValidator.SetInt(levelEvent, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, float? value)
        {
            if (!value.HasValue)
                return Disable(levelEvent, key);
            VisualEventValidator.SetFloat(levelEvent, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent levelEvent, string key, VisualVector2? value)
        {
            if (value == null)
                return Disable(levelEvent, key);
            VisualEventValidator.SetVector2(levelEvent, key, value);
            return 1;
        }

        private static int SetOptional<T>(LevelEvent levelEvent, string key, T? value)
            where T : struct, Enum
        {
            if (!value.HasValue)
                return Disable(levelEvent, key);
            VisualEventValidator.SetEnum(levelEvent, key, value.Value);
            return 1;
        }

        private static int Disable(LevelEvent levelEvent, string key)
        {
            VisualEventValidator.Disable(levelEvent, key);
            return 0;
        }
    }
}
