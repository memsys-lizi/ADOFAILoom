using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class TrackEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public TrackEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddColorAsync(
            string revision,
            ColorTrackEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateColor, token);

        public Task<VisualEventMutationResult> UpdateColorAsync(
            string revision,
            ColorTrackEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.ColorTrack, UpdateColor, token);

        public Task<VisualEventMutationResult> RemoveColorAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.ColorTrack, token);

        public Task<VisualEventMutationResult> AddAnimateAsync(
            string revision,
            AnimateTrackEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateAnimate, token);

        public Task<VisualEventMutationResult> UpdateAnimateAsync(
            string revision,
            AnimateTrackEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.AnimateTrack, UpdateAnimate, token);

        public Task<VisualEventMutationResult> RemoveAnimateAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.AnimateTrack, token);

        public Task<VisualEventMutationResult> AddRecolorAsync(
            string revision,
            RecolorTrackEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateRecolor, token);

        public Task<VisualEventMutationResult> UpdateRecolorAsync(
            string revision,
            RecolorTrackEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.RecolorTrack, UpdateRecolor, token);

        public Task<VisualEventMutationResult> RemoveRecolorAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.RecolorTrack, token);

        public Task<VisualEventMutationResult> AddMoveAsync(
            string revision,
            MoveTrackEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateMove, token);

        public Task<VisualEventMutationResult> UpdateMoveAsync(
            string revision,
            MoveTrackEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.MoveTrack, UpdateMove, token);

        public Task<VisualEventMutationResult> RemoveMoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.MoveTrack, token);

        public Task<VisualEventMutationResult> AddPositionAsync(
            string revision,
            PositionTrackEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreatePosition, token);

        public Task<VisualEventMutationResult> UpdatePositionAsync(
            string revision,
            PositionTrackEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.PositionTrack, UpdatePosition, token);

        public Task<VisualEventMutationResult> RemovePositionAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.PositionTrack, token);

        public Task<VisualEventMutationResult> AddDimensionsAsync(
            string revision,
            TileDimensionsEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateDimensions, token);

        public Task<VisualEventMutationResult> UpdateDimensionsAsync(
            string revision,
            TileDimensionsEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.TileDimensions, UpdateDimensions, token);

        public Task<VisualEventMutationResult> RemoveDimensionsAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.TileDimensions, token);

        public Task<VisualEventMutationResult> AddIconAsync(
            string revision,
            SetFloorIconEventCreate[] events,
            CancellationToken token
        ) => Add(revision, events, CreateIcon, token);

        public Task<VisualEventMutationResult> UpdateIconAsync(
            string revision,
            SetFloorIconEventUpdate[] events,
            CancellationToken token
        ) => Update(revision, events, LevelEventType.SetFloorIcon, UpdateIcon, token);

        public Task<VisualEventMutationResult> RemoveIconAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => Remove(revision, references, LevelEventType.SetFloorIcon, token);

        private Task<VisualEventMutationResult> Add<T>(
            string revision,
            T[] events,
            Func<scnEditor, T, LevelEvent> create,
            CancellationToken token
        ) => engine.AddAsync(revision, events, create, token, RefreshTracks);

        private Task<VisualEventMutationResult> Update<T>(
            string revision,
            T[] events,
            LevelEventType type,
            Func<scnEditor, LevelEvent, T, LevelEvent> apply,
            CancellationToken token
        )
            where T : FloorEventUpdate =>
            engine.UpdateAsync(
                revision,
                events,
                type,
                item => item.Reference,
                apply,
                token,
                RefreshTracks
            );

        private Task<VisualEventMutationResult> Remove(
            string revision,
            VisualEventReference[] references,
            LevelEventType type,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, type, token, RefreshTracks);

        private static LevelEvent CreateColor(scnEditor editor, ColorTrackEventCreate input)
        {
            LevelEvent e = CreateFloor(editor, LevelEventType.ColorTrack, input);
            SetTrackColors(
                e,
                input.TrackColorType,
                input.TrackColor,
                input.SecondaryTrackColor,
                input.TrackColorAnimationDuration,
                input.TrackColorPulse,
                input.TrackPulseLength,
                input.TrackStyle,
                input.TrackGlowIntensity
            );
            EditorAssetValidator.ValidateExistingLevelAsset(input.TrackTexture);
            VisualEventValidator.SetFile(e, "trackTexture", input.TrackTexture);
            VisualEventValidator.SetFloat(e, "trackTextureScale", input.TrackTextureScale);
            SetOptional(e, "floorIconOutlines", input.FloorIconOutlines);
            VisualEventValidator.SetBool(e, "justThisTile", input.JustThisTile);
            return e;
        }

        private static LevelEvent UpdateColor(
            scnEditor editor,
            LevelEvent e,
            ColorTrackEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
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
            if (input.TrackColorPulse.HasValue)
                VisualEventValidator.SetEnum(e, "trackColorPulse", input.TrackColorPulse.Value);
            if (input.TrackPulseLength.HasValue)
                VisualEventValidator.SetInt(e, "trackPulseLength", input.TrackPulseLength.Value);
            if (input.TrackStyle.HasValue)
                VisualEventValidator.SetEnum(e, "trackStyle", input.TrackStyle.Value);
            if (input.TrackTexture != null)
            {
                EditorAssetValidator.ValidateExistingLevelAsset(input.TrackTexture);
                VisualEventValidator.SetFile(e, "trackTexture", input.TrackTexture);
            }
            if (input.TrackTextureScale.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackTextureScale",
                    input.TrackTextureScale.Value
                );
            if (input.TrackGlowIntensity.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackGlowIntensity",
                    input.TrackGlowIntensity.Value
                );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.FloorIconOutlines.HasValue,
                ColorTrackProperty.FloorIconOutlines,
                "floorIconOutlines"
            );
            if (input.FloorIconOutlines.HasValue)
                VisualEventValidator.SetBool(e, "floorIconOutlines", input.FloorIconOutlines.Value);
            if (input.JustThisTile.HasValue)
                VisualEventValidator.SetBool(e, "justThisTile", input.JustThisTile.Value);
            return e;
        }

        private static LevelEvent CreateAnimate(scnEditor editor, AnimateTrackEventCreate input)
        {
            LevelEvent e = CreateFloor(editor, LevelEventType.AnimateTrack, input);
            SetOptional(e, "trackAnimation", input.TrackAnimation);
            VisualEventValidator.SetFloat(e, "beatsAhead", input.BeatsAhead);
            SetOptional(e, "trackDisappearAnimation", input.TrackDisappearAnimation);
            VisualEventValidator.SetFloat(e, "beatsBehind", input.BeatsBehind);
            if (!input.TrackAnimation.HasValue && !input.TrackDisappearAnimation.HasValue)
                throw new ArgumentException("AnimateTrack must enable at least one animation.");
            return e;
        }

        private static LevelEvent UpdateAnimate(
            scnEditor editor,
            LevelEvent e,
            AnimateTrackEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.TrackAnimation.HasValue,
                AnimateTrackProperty.TrackAnimation,
                "trackAnimation"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.TrackDisappearAnimation.HasValue,
                AnimateTrackProperty.TrackDisappearAnimation,
                "trackDisappearAnimation"
            );
            if (input.TrackAnimation.HasValue)
                VisualEventValidator.SetEnum(e, "trackAnimation", input.TrackAnimation.Value);
            if (input.BeatsAhead.HasValue)
                VisualEventValidator.SetFloat(e, "beatsAhead", input.BeatsAhead.Value);
            if (input.TrackDisappearAnimation.HasValue)
                VisualEventValidator.SetEnum(
                    e,
                    "trackDisappearAnimation",
                    input.TrackDisappearAnimation.Value
                );
            if (input.BeatsBehind.HasValue)
                VisualEventValidator.SetFloat(e, "beatsBehind", input.BeatsBehind.Value);
            return e;
        }

        private static LevelEvent CreateRecolor(scnEditor editor, RecolorTrackEventCreate input)
        {
            LevelEvent e = CreateTween(editor, LevelEventType.RecolorTrack, input);
            SetRange(e, input.StartTile, input.EndTile, input.GapLength);
            SetTrackColors(
                e,
                input.TrackColorType,
                input.TrackColor,
                input.SecondaryTrackColor,
                input.TrackColorAnimationDuration,
                input.TrackColorPulse,
                input.TrackPulseLength,
                input.TrackStyle,
                input.TrackGlowIntensity
            );
            return e;
        }

        private static LevelEvent UpdateRecolor(
            scnEditor editor,
            LevelEvent e,
            RecolorTrackEventUpdate input
        )
        {
            ApplyTween(editor, e, input);
            ApplyRange(e, input.StartTile, input.EndTile, input.GapLength);
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
            if (input.TrackColorPulse.HasValue)
                VisualEventValidator.SetEnum(e, "trackColorPulse", input.TrackColorPulse.Value);
            if (input.TrackPulseLength.HasValue)
                VisualEventValidator.SetInt(e, "trackPulseLength", input.TrackPulseLength.Value);
            if (input.TrackStyle.HasValue)
                VisualEventValidator.SetEnum(e, "trackStyle", input.TrackStyle.Value);
            if (input.TrackGlowIntensity.HasValue)
                VisualEventValidator.SetFloat(
                    e,
                    "trackGlowIntensity",
                    input.TrackGlowIntensity.Value
                );
            return e;
        }

        private static LevelEvent CreateMove(scnEditor editor, MoveTrackEventCreate input)
        {
            LevelEvent e = CreateTween(editor, LevelEventType.MoveTrack, input);
            SetRange(e, input.StartTile, input.EndTile, input.GapLength);
            int count = 0;
            count += SetOptional(e, "positionOffset", input.PositionOffset);
            count += SetOptional(e, "rotationOffset", input.RotationOffset);
            count += SetOptional(e, "scale", input.Scale);
            count += SetOptional(e, "opacity", input.Opacity);
            if (count == 0)
                throw new ArgumentException("MoveTrack must set at least one transform property.");
            VisualEventValidator.SetBool(e, "maxVfxOnly", input.MaximumVfxOnly);
            return e;
        }

        private static LevelEvent UpdateMove(
            scnEditor editor,
            LevelEvent e,
            MoveTrackEventUpdate input
        )
        {
            ApplyTween(editor, e, input);
            ApplyRange(e, input.StartTile, input.EndTile, input.GapLength);
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.PositionOffset != null,
                MoveTrackProperty.PositionOffset,
                "positionOffset"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.RotationOffset.HasValue,
                MoveTrackProperty.RotationOffset,
                "rotationOffset"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.Scale != null,
                MoveTrackProperty.Scale,
                "scale"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.Opacity.HasValue,
                MoveTrackProperty.Opacity,
                "opacity"
            );
            if (input.PositionOffset != null)
                VisualEventValidator.SetVector2(e, "positionOffset", input.PositionOffset);
            if (input.RotationOffset.HasValue)
                VisualEventValidator.SetFloat(e, "rotationOffset", input.RotationOffset.Value);
            if (input.Scale != null)
                VisualEventValidator.SetVector2(e, "scale", input.Scale);
            if (input.Opacity.HasValue)
                VisualEventValidator.SetFloat(e, "opacity", input.Opacity.Value);
            if (input.MaximumVfxOnly.HasValue)
                VisualEventValidator.SetBool(e, "maxVfxOnly", input.MaximumVfxOnly.Value);
            return e;
        }

        private static LevelEvent CreatePosition(scnEditor editor, PositionTrackEventCreate input)
        {
            LevelEvent e = CreateFloor(editor, LevelEventType.PositionTrack, input);
            SetOptional(e, "positionOffset", input.PositionOffset);
            VisualEventValidator.SetTile(e, "relativeTo", input.RelativeTo);
            SetOptional(e, "rotation", input.Rotation);
            SetOptional(e, "scale", input.Scale);
            SetOptional(e, "opacity", input.Opacity);
            VisualEventValidator.SetBool(e, "justThisTile", input.JustThisTile);
            VisualEventValidator.SetBool(e, "editorOnly", input.EditorOnly);
            SetOptional(e, "stickToFloors", input.StickToFloors);
            return e;
        }

        private static LevelEvent UpdatePosition(
            scnEditor editor,
            LevelEvent e,
            PositionTrackEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.PositionOffset != null,
                PositionTrackProperty.PositionOffset,
                "positionOffset"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.Rotation.HasValue,
                PositionTrackProperty.Rotation,
                "rotation"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.Scale.HasValue,
                PositionTrackProperty.Scale,
                "scale"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.Opacity.HasValue,
                PositionTrackProperty.Opacity,
                "opacity"
            );
            ApplyDisabled(
                e,
                input.DisabledProperties,
                input.StickToFloors.HasValue,
                PositionTrackProperty.StickToFloors,
                "stickToFloors"
            );
            if (input.PositionOffset != null)
                VisualEventValidator.SetVector2(e, "positionOffset", input.PositionOffset);
            if (input.RelativeTo != null)
                VisualEventValidator.SetTile(e, "relativeTo", input.RelativeTo);
            if (input.Rotation.HasValue)
                VisualEventValidator.SetFloat(e, "rotation", input.Rotation.Value);
            if (input.Scale.HasValue)
                VisualEventValidator.SetFloat(e, "scale", input.Scale.Value);
            if (input.Opacity.HasValue)
                VisualEventValidator.SetFloat(e, "opacity", input.Opacity.Value);
            if (input.JustThisTile.HasValue)
                VisualEventValidator.SetBool(e, "justThisTile", input.JustThisTile.Value);
            if (input.EditorOnly.HasValue)
                VisualEventValidator.SetBool(e, "editorOnly", input.EditorOnly.Value);
            if (input.StickToFloors.HasValue)
                VisualEventValidator.SetBool(e, "stickToFloors", input.StickToFloors.Value);
            return e;
        }

        private static LevelEvent CreateDimensions(
            scnEditor editor,
            TileDimensionsEventCreate input
        )
        {
            LevelEvent e = CreateFloor(editor, LevelEventType.TileDimensions, input);
            VisualEventValidator.SetFloat(e, "width", input.Width);
            VisualEventValidator.SetFloat(e, "length", input.Length);
            return e;
        }

        private static LevelEvent UpdateDimensions(
            scnEditor editor,
            LevelEvent e,
            TileDimensionsEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            if (input.Width.HasValue)
                VisualEventValidator.SetFloat(e, "width", input.Width.Value);
            if (input.Length.HasValue)
                VisualEventValidator.SetFloat(e, "length", input.Length.Value);
            return e;
        }

        private static LevelEvent CreateIcon(scnEditor editor, SetFloorIconEventCreate input)
        {
            LevelEvent e = CreateFloor(editor, LevelEventType.SetFloorIcon, input);
            VisualEventValidator.SetEnum(e, "icon", input.Icon);
            return e;
        }

        private static LevelEvent UpdateIcon(
            scnEditor editor,
            LevelEvent e,
            SetFloorIconEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            if (input.Icon.HasValue)
                VisualEventValidator.SetEnum(e, "icon", input.Icon.Value);
            return e;
        }

        private static LevelEvent CreateFloor(
            scnEditor editor,
            LevelEventType type,
            FloorEventCreate input
        )
        {
            var e = new LevelEvent(input.Floor, type);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            return e;
        }

        private static LevelEvent CreateTween(
            scnEditor editor,
            LevelEventType type,
            TweenEventCreate input
        )
        {
            LevelEvent e = CreateFloor(editor, type, input);
            VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset);
            VisualEventValidator.SetFloat(e, "duration", input.Duration);
            VisualEventValidator.SetEnum(e, "ease", input.Ease);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
            return e;
        }

        private static void ApplyFloor(scnEditor editor, LevelEvent e, FloorEventUpdate input)
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, input.Floor.Value);
                e.floor = input.Floor.Value;
            }
        }

        private static void ApplyTween(scnEditor editor, LevelEvent e, TweenEventUpdate input)
        {
            ApplyFloor(editor, e, input);
            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset.Value);
            if (input.Duration.HasValue)
                VisualEventValidator.SetFloat(e, "duration", input.Duration.Value);
            if (input.Ease.HasValue)
                VisualEventValidator.SetEnum(e, "ease", input.Ease.Value);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
        }

        private static void SetRange(
            LevelEvent e,
            VisualTileReference start,
            VisualTileReference end,
            int gap
        )
        {
            VisualEventValidator.SetTile(e, "startTile", start);
            VisualEventValidator.SetTile(e, "endTile", end);
            VisualEventValidator.SetInt(e, "gapLength", gap);
        }

        private static void ApplyRange(
            LevelEvent e,
            VisualTileReference? start,
            VisualTileReference? end,
            int? gap
        )
        {
            if (start != null)
                VisualEventValidator.SetTile(e, "startTile", start);
            if (end != null)
                VisualEventValidator.SetTile(e, "endTile", end);
            if (gap.HasValue)
                VisualEventValidator.SetInt(e, "gapLength", gap.Value);
        }

        private static void SetTrackColors(
            LevelEvent e,
            VisualTrackColorType type,
            string color,
            string secondary,
            float animationDuration,
            VisualTrackColorPulse pulse,
            int pulseLength,
            VisualTrackStyle style,
            float glow
        )
        {
            VisualEventValidator.SetEnum(e, "trackColorType", type);
            VisualEventValidator.SetColor(e, "trackColor", color);
            VisualEventValidator.SetColor(e, "secondaryTrackColor", secondary);
            VisualEventValidator.SetFloat(e, "trackColorAnimDuration", animationDuration);
            VisualEventValidator.SetEnum(e, "trackColorPulse", pulse);
            VisualEventValidator.SetInt(e, "trackPulseLength", pulseLength);
            VisualEventValidator.SetEnum(e, "trackStyle", style);
            VisualEventValidator.SetFloat(e, "trackGlowIntensity", glow);
        }

        private static int SetOptional(LevelEvent e, string key, VisualVector2? value)
        {
            if (value == null)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetVector2(e, key, value);
            return 1;
        }

        private static int SetOptional(LevelEvent e, string key, float? value)
        {
            if (!value.HasValue)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetFloat(e, key, value.Value);
            return 1;
        }

        private static int SetOptional(LevelEvent e, string key, bool? value)
        {
            if (!value.HasValue)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetBool(e, key, value.Value);
            return 1;
        }

        private static int SetOptional<T>(LevelEvent e, string key, T? value)
            where T : struct, Enum
        {
            if (!value.HasValue)
            {
                VisualEventValidator.Disable(e, key);
                return 0;
            }
            VisualEventValidator.SetEnum(e, key, value.Value);
            return 1;
        }

        private static void ApplyDisabled<T>(
            LevelEvent e,
            IEnumerable<T>? disabled,
            bool hasValue,
            T property,
            string key
        )
            where T : struct, Enum
        {
            if (disabled == null)
                return;
            var seen = new HashSet<T>();
            foreach (T item in disabled)
            {
                if (!seen.Add(item))
                    throw new ArgumentException($"Property '{item}' was disabled more than once.");
                if (EqualityComparer<T>.Default.Equals(item, property))
                {
                    if (hasValue)
                        throw new ArgumentException(
                            $"Property '{item}' cannot be updated and disabled together."
                        );
                    VisualEventValidator.Disable(e, key);
                }
            }
        }

        private static void RefreshTracks(scnEditor editor) =>
            ADOBase.customLevel.UpdateFloorSprites();
    }
}
