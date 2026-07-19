using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class BackgroundEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public BackgroundEventActions(VisualEventMutationEngine engine)
        {
            this.engine = engine;
        }

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            CustomBackgroundEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token, RefreshBackground);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            CustomBackgroundEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.CustomBackground,
                item => item.Reference,
                Update,
                token,
                RefreshBackground
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) =>
            engine.RemoveAsync(
                revision,
                references,
                LevelEventType.CustomBackground,
                token,
                RefreshBackground
            );

        private static LevelEvent Create(scnEditor editor, CustomBackgroundEventCreate input)
        {
            var levelEvent = new LevelEvent(input.Floor, LevelEventType.CustomBackground);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            ApplyOffset(levelEvent, input.AngleOffset, input.EventTag);
            SetImage(levelEvent, input.BackgroundImage);
            VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            VisualEventValidator.SetColor(levelEvent, "imageColor", input.ImageColor);
            VisualEventValidator.SetVector2(levelEvent, "parallax", input.Parallax);
            VisualEventValidator.SetEnum(levelEvent, "bgDisplayMode", input.DisplayMode);
            VisualEventValidator.SetBool(levelEvent, "imageSmoothing", input.ImageSmoothing);
            VisualEventValidator.SetBool(levelEvent, "lockRot", input.LockRotation);
            VisualEventValidator.SetBool(levelEvent, "loopBG", input.Loop);
            VisualEventValidator.SetInt(levelEvent, "scalingRatio", input.ScalingRatio);
            return levelEvent;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent levelEvent,
            CustomBackgroundEventUpdate input
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
            if (input.BackgroundImage != null)
                SetImage(levelEvent, input.BackgroundImage);
            if (input.Color != null)
                VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            if (input.ImageColor != null)
                VisualEventValidator.SetColor(levelEvent, "imageColor", input.ImageColor);
            if (input.Parallax != null)
                VisualEventValidator.SetVector2(levelEvent, "parallax", input.Parallax);
            if (input.DisplayMode.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "bgDisplayMode", input.DisplayMode.Value);
            if (input.ImageSmoothing.HasValue)
                VisualEventValidator.SetBool(
                    levelEvent,
                    "imageSmoothing",
                    input.ImageSmoothing.Value
                );
            if (input.LockRotation.HasValue)
                VisualEventValidator.SetBool(levelEvent, "lockRot", input.LockRotation.Value);
            if (input.Loop.HasValue)
                VisualEventValidator.SetBool(levelEvent, "loopBG", input.Loop.Value);
            if (input.ScalingRatio.HasValue)
                VisualEventValidator.SetInt(levelEvent, "scalingRatio", input.ScalingRatio.Value);
            return levelEvent;
        }

        private static void ApplyOffset(LevelEvent levelEvent, float angleOffset, string? eventTag)
        {
            VisualEventValidator.SetFloat(levelEvent, "angleOffset", angleOffset);
            if (eventTag != null)
                VisualEventValidator.SetString(levelEvent, "eventTag", eventTag);
        }

        private static void SetImage(LevelEvent levelEvent, string fileName)
        {
            EditorAssetValidator.ValidateExistingLevelAsset(fileName);
            VisualEventValidator.SetFile(levelEvent, "bgImage", fileName);
        }

        private static void RefreshBackground(scnEditor editor)
        {
            ADOBase.customLevel.UpdateBackgroundSprites();
        }
    }
}
