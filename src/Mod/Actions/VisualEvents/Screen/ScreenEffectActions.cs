using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class ScreenEffectActions
    {
        private readonly VisualEventMutationEngine engine;

        public ScreenEffectActions(VisualEventMutationEngine engine)
        {
            this.engine = engine;
        }

        public Task<VisualEventMutationResult> AddFlashAsync(
            string revision,
            FlashEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateFlash, token);

        public Task<VisualEventMutationResult> UpdateFlashAsync(
            string revision,
            FlashEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.Flash,
                item => item.Reference,
                UpdateFlash,
                token
            );

        public Task<VisualEventMutationResult> RemoveFlashAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.Flash, token);

        public Task<VisualEventMutationResult> AddBloomAsync(
            string revision,
            BloomEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateBloom, token);

        public Task<VisualEventMutationResult> UpdateBloomAsync(
            string revision,
            BloomEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.Bloom,
                item => item.Reference,
                UpdateBloom,
                token
            );

        public Task<VisualEventMutationResult> RemoveBloomAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.Bloom, token);

        public Task<VisualEventMutationResult> AddShakeScreenAsync(
            string revision,
            ShakeScreenEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateShakeScreen, token);

        public Task<VisualEventMutationResult> UpdateShakeScreenAsync(
            string revision,
            ShakeScreenEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.ShakeScreen,
                item => item.Reference,
                UpdateShakeScreen,
                token
            );

        public Task<VisualEventMutationResult> RemoveShakeScreenAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.ShakeScreen, token);

        public Task<VisualEventMutationResult> AddScreenTileAsync(
            string revision,
            ScreenTileEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateScreenTile, token);

        public Task<VisualEventMutationResult> UpdateScreenTileAsync(
            string revision,
            ScreenTileEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.ScreenTile,
                item => item.Reference,
                UpdateScreenTile,
                token
            );

        public Task<VisualEventMutationResult> RemoveScreenTileAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.ScreenTile, token);

        public Task<VisualEventMutationResult> AddScreenScrollAsync(
            string revision,
            ScreenScrollEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateScreenScroll, token);

        public Task<VisualEventMutationResult> UpdateScreenScrollAsync(
            string revision,
            ScreenScrollEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.ScreenScroll,
                item => item.Reference,
                UpdateScreenScroll,
                token
            );

        public Task<VisualEventMutationResult> RemoveScreenScrollAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.ScreenScroll, token);

        public Task<VisualEventMutationResult> AddHallOfMirrorsAsync(
            string revision,
            HallOfMirrorsEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateHallOfMirrors, token);

        public Task<VisualEventMutationResult> UpdateHallOfMirrorsAsync(
            string revision,
            HallOfMirrorsEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.HallOfMirrors,
                item => item.Reference,
                UpdateHallOfMirrors,
                token
            );

        public Task<VisualEventMutationResult> RemoveHallOfMirrorsAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.HallOfMirrors, token);

        public Task<VisualEventMutationResult> AddSetFrameRateAsync(
            string revision,
            SetFrameRateEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, CreateSetFrameRate, token);

        public Task<VisualEventMutationResult> UpdateSetFrameRateAsync(
            string revision,
            SetFrameRateEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.SetFrameRate,
                item => item.Reference,
                UpdateSetFrameRate,
                token
            );

        public Task<VisualEventMutationResult> RemoveSetFrameRateAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.SetFrameRate, token);

        private static LevelEvent CreateFlash(scnEditor editor, FlashEventCreate input)
        {
            LevelEvent levelEvent = CreateTween(editor, LevelEventType.Flash, input);
            VisualEventValidator.SetEnum(levelEvent, "plane", input.Plane);
            VisualEventValidator.SetColor(levelEvent, "startColor", input.StartColor);
            VisualEventValidator.SetFloat(levelEvent, "startOpacity", input.StartOpacity);
            VisualEventValidator.SetColor(levelEvent, "endColor", input.EndColor);
            VisualEventValidator.SetFloat(levelEvent, "endOpacity", input.EndOpacity);
            return levelEvent;
        }

        private static LevelEvent UpdateFlash(
            scnEditor editor,
            LevelEvent levelEvent,
            FlashEventUpdate input
        )
        {
            ApplyTween(editor, levelEvent, input);
            if (input.Plane.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "plane", input.Plane.Value);
            if (input.StartColor != null)
                VisualEventValidator.SetColor(levelEvent, "startColor", input.StartColor);
            if (input.StartOpacity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "startOpacity", input.StartOpacity.Value);
            if (input.EndColor != null)
                VisualEventValidator.SetColor(levelEvent, "endColor", input.EndColor);
            if (input.EndOpacity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "endOpacity", input.EndOpacity.Value);
            return levelEvent;
        }

        private static LevelEvent CreateBloom(scnEditor editor, BloomEventCreate input)
        {
            LevelEvent levelEvent = CreateTween(editor, LevelEventType.Bloom, input);
            VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled);
            VisualEventValidator.SetFloat(levelEvent, "threshold", input.Threshold);
            VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity);
            VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            return levelEvent;
        }

        private static LevelEvent UpdateBloom(
            scnEditor editor,
            LevelEvent levelEvent,
            BloomEventUpdate input
        )
        {
            ApplyTween(editor, levelEvent, input);
            if (input.Enabled.HasValue)
                VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled.Value);
            if (input.Threshold.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "threshold", input.Threshold.Value);
            if (input.Intensity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity.Value);
            if (input.Color != null)
                VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            return levelEvent;
        }

        private static LevelEvent CreateShakeScreen(scnEditor editor, ShakeScreenEventCreate input)
        {
            LevelEvent levelEvent = CreateTween(editor, LevelEventType.ShakeScreen, input);
            VisualEventValidator.SetFloat(levelEvent, "strength", input.Strength);
            VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity);
            VisualEventValidator.SetBool(levelEvent, "fadeOut", input.FadeOut);
            return levelEvent;
        }

        private static LevelEvent UpdateShakeScreen(
            scnEditor editor,
            LevelEvent levelEvent,
            ShakeScreenEventUpdate input
        )
        {
            ApplyTween(editor, levelEvent, input);
            if (input.Strength.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "strength", input.Strength.Value);
            if (input.Intensity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "intensity", input.Intensity.Value);
            if (input.FadeOut.HasValue)
                VisualEventValidator.SetBool(levelEvent, "fadeOut", input.FadeOut.Value);
            return levelEvent;
        }

        private static LevelEvent CreateScreenTile(scnEditor editor, ScreenTileEventCreate input)
        {
            LevelEvent levelEvent = CreateTween(editor, LevelEventType.ScreenTile, input);
            VisualEventValidator.SetVector2(levelEvent, "tile", input.Tile);
            return levelEvent;
        }

        private static LevelEvent UpdateScreenTile(
            scnEditor editor,
            LevelEvent levelEvent,
            ScreenTileEventUpdate input
        )
        {
            ApplyTween(editor, levelEvent, input);
            if (input.Tile != null)
                VisualEventValidator.SetVector2(levelEvent, "tile", input.Tile);
            return levelEvent;
        }

        private static LevelEvent CreateScreenScroll(
            scnEditor editor,
            ScreenScrollEventCreate input
        )
        {
            LevelEvent levelEvent = CreateOffset(editor, LevelEventType.ScreenScroll, input);
            VisualEventValidator.SetVector2(levelEvent, "scroll", input.Scroll);
            return levelEvent;
        }

        private static LevelEvent UpdateScreenScroll(
            scnEditor editor,
            LevelEvent levelEvent,
            ScreenScrollEventUpdate input
        )
        {
            ApplyOffset(editor, levelEvent, input);
            if (input.Scroll != null)
                VisualEventValidator.SetVector2(levelEvent, "scroll", input.Scroll);
            return levelEvent;
        }

        private static LevelEvent CreateHallOfMirrors(
            scnEditor editor,
            HallOfMirrorsEventCreate input
        )
        {
            LevelEvent levelEvent = CreateOffset(editor, LevelEventType.HallOfMirrors, input);
            VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled);
            return levelEvent;
        }

        private static LevelEvent UpdateHallOfMirrors(
            scnEditor editor,
            LevelEvent levelEvent,
            HallOfMirrorsEventUpdate input
        )
        {
            ApplyOffset(editor, levelEvent, input);
            if (input.Enabled.HasValue)
                VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled.Value);
            return levelEvent;
        }

        private static LevelEvent CreateSetFrameRate(
            scnEditor editor,
            SetFrameRateEventCreate input
        )
        {
            LevelEvent levelEvent = CreateOffset(editor, LevelEventType.SetFrameRate, input);
            VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled);
            VisualEventValidator.SetFloat(levelEvent, "frameRate", input.FrameRate);
            return levelEvent;
        }

        private static LevelEvent UpdateSetFrameRate(
            scnEditor editor,
            LevelEvent levelEvent,
            SetFrameRateEventUpdate input
        )
        {
            ApplyOffset(editor, levelEvent, input);
            if (input.Enabled.HasValue)
                VisualEventValidator.SetBool(levelEvent, "enabled", input.Enabled.Value);
            if (input.FrameRate.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "frameRate", input.FrameRate.Value);
            return levelEvent;
        }

        private static LevelEvent CreateTween(
            scnEditor editor,
            LevelEventType type,
            TweenEventCreate input
        )
        {
            LevelEvent levelEvent = CreateOffset(editor, type, input);
            VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration);
            VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease);
            return levelEvent;
        }

        private static LevelEvent CreateOffset(
            scnEditor editor,
            LevelEventType type,
            OffsetEventCreate input
        )
        {
            var levelEvent = new LevelEvent(input.Floor, type);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset);
            if (input.EventTag != null)
            {
                VisualEventValidator.SetString(levelEvent, "eventTag", input.EventTag);
            }

            return levelEvent;
        }

        private static void ApplyTween(
            scnEditor editor,
            LevelEvent levelEvent,
            TweenEventUpdate input
        )
        {
            ApplyOffset(editor, levelEvent, input);
            if (input.Duration.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "duration", input.Duration.Value);
            if (input.Ease.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "ease", input.Ease.Value);
        }

        private static void ApplyOffset(
            scnEditor editor,
            LevelEvent levelEvent,
            OffsetEventUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor.Value);
                levelEvent.floor = input.Floor.Value;
            }

            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "angleOffset", input.AngleOffset.Value);
            if (input.EventTag != null)
                VisualEventValidator.SetString(levelEvent, "eventTag", input.EventTag);
        }
    }
}
