using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class ObjectDecorationActions
    {
        private readonly DecorationMutationEngine engine;

        public ObjectDecorationActions(DecorationMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            ObjectDecorationCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            ObjectDecorationUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.AddObject,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.AddObject, token);

        private static LevelEvent Create(scnEditor editor, ObjectDecorationCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.AddObject);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            VisualEventValidator.SetEnum(e, "objectType", input.ObjectType);
            SetTag(e, input.Tag);
            SetCommon(e, input);
            ValidateCreateGroups(input);
            if (input.ObjectType == VisualObjectDecorationType.PlayerBubble)
                SetBubble(
                    e,
                    input.BubbleAppearStartOffset!.Value,
                    input.BubbleAppearEndOffset!.Value,
                    input.BubbleDisappearOffset!.Value,
                    input.BubbleSpawnOffset!.Value
                );
            if (input.ObjectType == VisualObjectDecorationType.Planet)
                SetPlanet(
                    e,
                    input.PlanetColorType!.Value,
                    input.PlanetColor!,
                    input.PlanetTailColor!
                );
            if (input.ObjectType == VisualObjectDecorationType.Floor)
                SetFloor(
                    e,
                    input.TrackType!.Value,
                    input.TrackAngle!.Value,
                    input.TrackColorType!.Value,
                    input.TrackColor!,
                    input.SecondaryTrackColor!,
                    input.TrackColorAnimationDuration!.Value,
                    input.TrackOpacity!.Value,
                    input.TrackStyle!.Value,
                    input.TrackIcon!.Value,
                    input.TrackIconAngle!.Value,
                    input.TrackIconFlipped!.Value,
                    input.TrackRedSwirl!.Value,
                    input.TrackGraySetSpeedIcon!.Value,
                    input.TrackSetSpeedIconBpm!.Value,
                    input.TrackGlowEnabled!.Value,
                    input.TrackGlowColor!,
                    input.TrackIconOutlines!.Value
                );
            return e;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent e,
            ObjectDecorationUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, input.Floor.Value);
                e.floor = input.Floor.Value;
            }
            if (input.Tag != null)
                SetTag(e, input.Tag);
            ApplyCommon(e, input);
            VisualObjectDecorationType type = Enum.Parse<VisualObjectDecorationType>(
                e["objectType"].ToString()
            );
            ValidateUpdateGroups(type, input);
            if (input.BubbleAppearStartOffset.HasValue)
                VisualEventValidator.SetInt(
                    e,
                    "bubbleAppearStartOffset",
                    input.BubbleAppearStartOffset.Value
                );
            if (input.BubbleAppearEndOffset.HasValue)
                VisualEventValidator.SetInt(
                    e,
                    "bubbleAppearEndOffset",
                    input.BubbleAppearEndOffset.Value
                );
            if (input.BubbleDisappearOffset.HasValue)
                VisualEventValidator.SetInt(
                    e,
                    "bubbleDisappearOffset",
                    input.BubbleDisappearOffset.Value
                );
            if (input.BubbleSpawnOffset.HasValue)
                VisualEventValidator.SetInt(e, "bubbleSpawnOffset", input.BubbleSpawnOffset.Value);
            if (input.PlanetColorType.HasValue)
                VisualEventValidator.SetEnum(e, "planetColorType", input.PlanetColorType.Value);
            if (input.PlanetColor != null)
                VisualEventValidator.SetColor(e, "planetColor", input.PlanetColor);
            if (input.PlanetTailColor != null)
                VisualEventValidator.SetColor(e, "planetTailColor", input.PlanetTailColor);
            ApplyFloorUpdate(e, input);
            return e;
        }

        private static void SetCommon(LevelEvent e, ObjectDecorationProperties input)
        {
            VisualEventValidator.SetVector2(e, "position", input.Position);
            VisualEventValidator.SetEnum(e, "relativeTo", input.RelativeTo);
            VisualEventValidator.SetVector2(e, "pivotOffset", input.PivotOffset);
            VisualEventValidator.SetFloat(e, "rotation", input.Rotation);
            VisualEventValidator.SetBool(e, "lockRotation", input.LockRotation);
            VisualEventValidator.SetVector2(e, "scale", input.Scale);
            VisualEventValidator.SetBool(e, "lockScale", input.LockScale);
            VisualEventValidator.SetInt(e, "depth", input.Depth);
            VisualEventValidator.SetBool(e, "syncFloorDepth", input.SyncFloorDepth);
            VisualEventValidator.SetVector2(e, "parallax", input.Parallax);
            VisualEventValidator.SetVector2(e, "parallaxOffset", input.ParallaxOffset);
        }

        private static void ApplyCommon(LevelEvent e, ObjectDecorationUpdate input)
        {
            if (input.Position != null)
                VisualEventValidator.SetVector2(e, "position", input.Position);
            if (input.RelativeTo.HasValue)
                VisualEventValidator.SetEnum(e, "relativeTo", input.RelativeTo.Value);
            if (input.PivotOffset != null)
                VisualEventValidator.SetVector2(e, "pivotOffset", input.PivotOffset);
            if (input.Rotation.HasValue)
                VisualEventValidator.SetFloat(e, "rotation", input.Rotation.Value);
            if (input.LockRotation.HasValue)
                VisualEventValidator.SetBool(e, "lockRotation", input.LockRotation.Value);
            if (input.Scale != null)
                VisualEventValidator.SetVector2(e, "scale", input.Scale);
            if (input.LockScale.HasValue)
                VisualEventValidator.SetBool(e, "lockScale", input.LockScale.Value);
            if (input.Depth.HasValue)
                VisualEventValidator.SetInt(e, "depth", input.Depth.Value);
            if (input.SyncFloorDepth.HasValue)
                VisualEventValidator.SetBool(e, "syncFloorDepth", input.SyncFloorDepth.Value);
            if (input.Parallax != null)
                VisualEventValidator.SetVector2(e, "parallax", input.Parallax);
            if (input.ParallaxOffset != null)
                VisualEventValidator.SetVector2(e, "parallaxOffset", input.ParallaxOffset);
        }

        private static void SetTag(LevelEvent e, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Object decoration tag cannot be empty or whitespace.");
            VisualEventValidator.SetString(e, "tag", tag);
        }

        private static void SetBubble(LevelEvent e, int start, int end, int disappear, int spawn)
        {
            VisualEventValidator.SetInt(e, "bubbleAppearStartOffset", start);
            VisualEventValidator.SetInt(e, "bubbleAppearEndOffset", end);
            VisualEventValidator.SetInt(e, "bubbleDisappearOffset", disappear);
            VisualEventValidator.SetInt(e, "bubbleSpawnOffset", spawn);
        }

        private static void SetPlanet(
            LevelEvent e,
            VisualPlanetColorType type,
            string color,
            string tail
        )
        {
            VisualEventValidator.SetEnum(e, "planetColorType", type);
            VisualEventValidator.SetColor(e, "planetColor", color);
            VisualEventValidator.SetColor(e, "planetTailColor", tail);
        }

        private static void SetFloor(
            LevelEvent e,
            VisualFloorObjectType type,
            float angle,
            VisualFloorColorType colorType,
            string color,
            string secondary,
            float colorDuration,
            float opacity,
            VisualTrackStyle style,
            VisualFloorIcon icon,
            float iconAngle,
            bool iconFlipped,
            bool redSwirl,
            bool graySpeed,
            float speedBpm,
            bool glowEnabled,
            string glowColor,
            bool outlines
        )
        {
            VisualEventValidator.SetEnum(e, "trackType", type);
            VisualEventValidator.SetFloat(e, "trackAngle", angle);
            VisualEventValidator.SetEnum(e, "trackColorType", colorType);
            VisualEventValidator.SetColor(e, "trackColor", color);
            VisualEventValidator.SetColor(e, "secondaryTrackColor", secondary);
            VisualEventValidator.SetFloat(e, "trackColorAnimDuration", colorDuration);
            VisualEventValidator.SetFloat(e, "trackOpacity", opacity);
            VisualEventValidator.SetEnum(e, "trackStyle", style);
            VisualEventValidator.SetEnum(e, "trackIcon", icon);
            VisualEventValidator.SetFloat(e, "trackIconAngle", iconAngle);
            VisualEventValidator.SetBool(e, "trackIconFlipped", iconFlipped);
            VisualEventValidator.SetBool(e, "trackRedSwirl", redSwirl);
            VisualEventValidator.SetBool(e, "trackGraySetSpeedIcon", graySpeed);
            VisualEventValidator.SetFloat(e, "trackSetSpeedIconBpm", speedBpm);
            VisualEventValidator.SetBool(e, "trackGlowEnabled", glowEnabled);
            VisualEventValidator.SetColor(e, "trackGlowColor", glowColor);
            VisualEventValidator.SetBool(e, "trackIconOutlines", outlines);
        }

        private static void ApplyFloorUpdate(LevelEvent e, ObjectDecorationUpdate i)
        {
            if (i.TrackType.HasValue)
                VisualEventValidator.SetEnum(e, "trackType", i.TrackType.Value);
            if (i.TrackAngle.HasValue)
                VisualEventValidator.SetFloat(e, "trackAngle", i.TrackAngle.Value);
            if (i.TrackColorType.HasValue)
                VisualEventValidator.SetEnum(e, "trackColorType", i.TrackColorType.Value);
            if (i.TrackColor != null)
                VisualEventValidator.SetColor(e, "trackColor", i.TrackColor);
            if (i.SecondaryTrackColor != null)
                VisualEventValidator.SetColor(e, "secondaryTrackColor", i.SecondaryTrackColor);
            if (i.TrackColorAnimationDuration.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackColorAnimDuration",
                    i.TrackColorAnimationDuration.Value
                );
            if (i.TrackOpacity.HasValue)
                VisualEventValidator.SetFloat(e, "trackOpacity", i.TrackOpacity.Value);
            if (i.TrackStyle.HasValue)
                VisualEventValidator.SetEnum(e, "trackStyle", i.TrackStyle.Value);
            if (i.TrackIcon.HasValue)
                VisualEventValidator.SetEnum(e, "trackIcon", i.TrackIcon.Value);
            if (i.TrackIconAngle.HasValue)
                VisualEventValidator.SetFloat(e, "trackIconAngle", i.TrackIconAngle.Value);
            if (i.TrackIconFlipped.HasValue)
                VisualEventValidator.SetBool(e, "trackIconFlipped", i.TrackIconFlipped.Value);
            if (i.TrackRedSwirl.HasValue)
                VisualEventValidator.SetBool(e, "trackRedSwirl", i.TrackRedSwirl.Value);
            if (i.TrackGraySetSpeedIcon.HasValue)
                VisualEventValidator.SetBool(
                    e,
                    "trackGraySetSpeedIcon",
                    i.TrackGraySetSpeedIcon.Value
                );
            if (i.TrackSetSpeedIconBpm.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackSetSpeedIconBpm",
                    i.TrackSetSpeedIconBpm.Value
                );
            if (i.TrackGlowEnabled.HasValue)
                VisualEventValidator.SetBool(e, "trackGlowEnabled", i.TrackGlowEnabled.Value);
            if (i.TrackGlowColor != null)
                VisualEventValidator.SetColor(e, "trackGlowColor", i.TrackGlowColor);
            if (i.TrackIconOutlines.HasValue)
                VisualEventValidator.SetBool(e, "trackIconOutlines", i.TrackIconOutlines.Value);
        }

        private static void ValidateCreateGroups(ObjectDecorationCreate input)
        {
            bool bubble = HasBubble(input);
            bool planet = HasPlanet(input);
            bool floor = HasFloor(input);
            if (
                input.ObjectType == VisualObjectDecorationType.PlayerBubble
                && (!AllBubble(input) || planet || floor)
            )
                throw new ArgumentException(
                    "PlayerBubble requires all bubble offsets and rejects planet or floor fields."
                );
            if (
                input.ObjectType == VisualObjectDecorationType.Planet
                && (!AllPlanet(input) || bubble || floor)
            )
                throw new ArgumentException(
                    "Planet requires all planet color fields and rejects bubble or floor fields."
                );
            if (
                input.ObjectType == VisualObjectDecorationType.Floor
                && (!AllFloor(input) || bubble || planet)
            )
                throw new ArgumentException(
                    "Floor requires all track appearance fields and rejects bubble or planet fields."
                );
        }

        private static void ValidateUpdateGroups(
            VisualObjectDecorationType type,
            ObjectDecorationUpdate input
        )
        {
            if (type != VisualObjectDecorationType.PlayerBubble && HasBubble(input))
                throw new ArgumentException(
                    "Bubble fields only apply to PlayerBubble decorations."
                );
            if (type != VisualObjectDecorationType.Planet && HasPlanet(input))
                throw new ArgumentException("Planet fields only apply to Planet decorations.");
            if (type != VisualObjectDecorationType.Floor && HasFloor(input))
                throw new ArgumentException("Track fields only apply to Floor decorations.");
        }

        private static bool HasBubble(ObjectDecorationCreate i) =>
            i.BubbleAppearStartOffset.HasValue
            || i.BubbleAppearEndOffset.HasValue
            || i.BubbleDisappearOffset.HasValue
            || i.BubbleSpawnOffset.HasValue;

        private static bool HasBubble(ObjectDecorationUpdate i) =>
            i.BubbleAppearStartOffset.HasValue
            || i.BubbleAppearEndOffset.HasValue
            || i.BubbleDisappearOffset.HasValue
            || i.BubbleSpawnOffset.HasValue;

        private static bool AllBubble(ObjectDecorationCreate i) =>
            i.BubbleAppearStartOffset.HasValue
            && i.BubbleAppearEndOffset.HasValue
            && i.BubbleDisappearOffset.HasValue
            && i.BubbleSpawnOffset.HasValue;

        private static bool HasPlanet(ObjectDecorationCreate i) =>
            i.PlanetColorType.HasValue || i.PlanetColor != null || i.PlanetTailColor != null;

        private static bool HasPlanet(ObjectDecorationUpdate i) =>
            i.PlanetColorType.HasValue || i.PlanetColor != null || i.PlanetTailColor != null;

        private static bool AllPlanet(ObjectDecorationCreate i) =>
            i.PlanetColorType.HasValue && i.PlanetColor != null && i.PlanetTailColor != null;

        private static bool HasFloor(ObjectDecorationCreate i) =>
            i.TrackType.HasValue
            || i.TrackAngle.HasValue
            || i.TrackColorType.HasValue
            || i.TrackColor != null
            || i.SecondaryTrackColor != null
            || i.TrackColorAnimationDuration.HasValue
            || i.TrackOpacity.HasValue
            || i.TrackStyle.HasValue
            || i.TrackIcon.HasValue
            || i.TrackIconAngle.HasValue
            || i.TrackIconFlipped.HasValue
            || i.TrackRedSwirl.HasValue
            || i.TrackGraySetSpeedIcon.HasValue
            || i.TrackSetSpeedIconBpm.HasValue
            || i.TrackGlowEnabled.HasValue
            || i.TrackGlowColor != null
            || i.TrackIconOutlines.HasValue;

        private static bool HasFloor(ObjectDecorationUpdate i) =>
            i.TrackType.HasValue
            || i.TrackAngle.HasValue
            || i.TrackColorType.HasValue
            || i.TrackColor != null
            || i.SecondaryTrackColor != null
            || i.TrackColorAnimationDuration.HasValue
            || i.TrackOpacity.HasValue
            || i.TrackStyle.HasValue
            || i.TrackIcon.HasValue
            || i.TrackIconAngle.HasValue
            || i.TrackIconFlipped.HasValue
            || i.TrackRedSwirl.HasValue
            || i.TrackGraySetSpeedIcon.HasValue
            || i.TrackSetSpeedIconBpm.HasValue
            || i.TrackGlowEnabled.HasValue
            || i.TrackGlowColor != null
            || i.TrackIconOutlines.HasValue;

        private static bool AllFloor(ObjectDecorationCreate i) =>
            i.TrackType.HasValue
            && i.TrackAngle.HasValue
            && i.TrackColorType.HasValue
            && i.TrackColor != null
            && i.SecondaryTrackColor != null
            && i.TrackColorAnimationDuration.HasValue
            && i.TrackOpacity.HasValue
            && i.TrackStyle.HasValue
            && i.TrackIcon.HasValue
            && i.TrackIconAngle.HasValue
            && i.TrackIconFlipped.HasValue
            && i.TrackRedSwirl.HasValue
            && i.TrackGraySetSpeedIcon.HasValue
            && i.TrackSetSpeedIconBpm.HasValue
            && i.TrackGlowEnabled.HasValue
            && i.TrackGlowColor != null
            && i.TrackIconOutlines.HasValue;
    }
}
