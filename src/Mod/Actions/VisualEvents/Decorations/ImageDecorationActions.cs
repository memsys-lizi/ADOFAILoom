using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class ImageDecorationActions
    {
        private readonly DecorationMutationEngine engine;

        public ImageDecorationActions(DecorationMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            ImageDecorationCreate[] events,
            CancellationToken cancellationToken
        ) => engine.AddAsync(revision, events, Create, cancellationToken);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            ImageDecorationUpdate[] events,
            CancellationToken cancellationToken
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.AddDecoration,
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
                LevelEventType.AddDecoration,
                cancellationToken
            );

        private static LevelEvent Create(scnEditor editor, ImageDecorationCreate input)
        {
            var levelEvent = new LevelEvent(input.Floor, LevelEventType.AddDecoration);
            VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor);
            SetImage(levelEvent, input.Image);
            VisualEventValidator.SetString(levelEvent, "tag", input.Tag);
            VisualEventValidator.SetVector2(levelEvent, "position", input.Position);
            VisualEventValidator.SetEnum(levelEvent, "relativeTo", input.RelativeTo);
            VisualEventValidator.SetBool(levelEvent, "stickToFloor", input.StickToFloor);
            VisualEventValidator.SetVector2(levelEvent, "pivotOffset", input.PivotOffset);
            VisualEventValidator.SetFloat(levelEvent, "rotation", input.Rotation);
            VisualEventValidator.SetBool(levelEvent, "lockRotation", input.LockRotation);
            VisualEventValidator.SetVector2(levelEvent, "scale", input.Scale);
            VisualEventValidator.SetFloat(levelEvent, "scaleMultiplier", input.ScaleMultiplier);
            VisualEventValidator.SetBool(levelEvent, "lockScale", input.LockScale);
            VisualEventValidator.SetVector2(levelEvent, "tile", input.Tile);
            VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            VisualEventValidator.SetFloat(levelEvent, "opacity", input.Opacity);
            VisualEventValidator.SetInt(levelEvent, "depth", input.Depth);
            VisualEventValidator.SetBool(levelEvent, "syncFloorDepth", input.SyncFloorDepth);
            VisualEventValidator.SetVector2(levelEvent, "parallax", input.Parallax);
            VisualEventValidator.SetVector2(levelEvent, "parallaxOffset", input.ParallaxOffset);
            VisualEventValidator.SetBool(levelEvent, "imageSmoothing", input.ImageSmoothing);
            VisualEventValidator.SetEnum(levelEvent, "blendMode", input.BlendMode);
            VisualEventValidator.SetEnum(levelEvent, "maskingType", input.MaskingType);
            VisualEventValidator.SetString(levelEvent, "maskingTarget", input.MaskingTarget);
            VisualEventValidator.SetBool(levelEvent, "useMaskingDepth", input.UseMaskingDepth);
            VisualEventValidator.SetInt(levelEvent, "maskingFrontDepth", input.MaskingFrontDepth);
            VisualEventValidator.SetInt(levelEvent, "maskingBackDepth", input.MaskingBackDepth);
            return levelEvent;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent levelEvent,
            ImageDecorationUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, levelEvent.info, input.Floor.Value);
                levelEvent.floor = input.Floor.Value;
            }

            if (input.Image != null)
                SetImage(levelEvent, input.Image);
            if (input.Tag != null)
                VisualEventValidator.SetString(levelEvent, "tag", input.Tag);
            if (input.Position != null)
                VisualEventValidator.SetVector2(levelEvent, "position", input.Position);
            if (input.RelativeTo.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "relativeTo", input.RelativeTo.Value);
            if (input.StickToFloor.HasValue)
                VisualEventValidator.SetBool(levelEvent, "stickToFloor", input.StickToFloor.Value);
            if (input.PivotOffset != null)
                VisualEventValidator.SetVector2(levelEvent, "pivotOffset", input.PivotOffset);
            if (input.Rotation.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "rotation", input.Rotation.Value);
            if (input.LockRotation.HasValue)
                VisualEventValidator.SetBool(levelEvent, "lockRotation", input.LockRotation.Value);
            if (input.Scale != null)
                VisualEventValidator.SetVector2(levelEvent, "scale", input.Scale);
            if (input.ScaleMultiplier.HasValue)
                VisualEventValidator.SetFloat(
                    levelEvent,
                    "scaleMultiplier",
                    input.ScaleMultiplier.Value
                );
            if (input.LockScale.HasValue)
                VisualEventValidator.SetBool(levelEvent, "lockScale", input.LockScale.Value);
            if (input.Tile != null)
                VisualEventValidator.SetVector2(levelEvent, "tile", input.Tile);
            if (input.Color != null)
                VisualEventValidator.SetColor(levelEvent, "color", input.Color);
            if (input.Opacity.HasValue)
                VisualEventValidator.SetFloat(levelEvent, "opacity", input.Opacity.Value);
            if (input.Depth.HasValue)
                VisualEventValidator.SetInt(levelEvent, "depth", input.Depth.Value);
            if (input.SyncFloorDepth.HasValue)
                VisualEventValidator.SetBool(
                    levelEvent,
                    "syncFloorDepth",
                    input.SyncFloorDepth.Value
                );
            if (input.Parallax != null)
                VisualEventValidator.SetVector2(levelEvent, "parallax", input.Parallax);
            if (input.ParallaxOffset != null)
                VisualEventValidator.SetVector2(levelEvent, "parallaxOffset", input.ParallaxOffset);
            if (input.ImageSmoothing.HasValue)
                VisualEventValidator.SetBool(
                    levelEvent,
                    "imageSmoothing",
                    input.ImageSmoothing.Value
                );
            if (input.BlendMode.HasValue)
                VisualEventValidator.SetEnum(levelEvent, "blendMode", input.BlendMode.Value);
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

        private static void SetImage(LevelEvent levelEvent, string image)
        {
            EditorAssetValidator.ValidateExistingLevelAsset(image);
            VisualEventValidator.SetFile(levelEvent, "decorationImage", image);
        }
    }
}
