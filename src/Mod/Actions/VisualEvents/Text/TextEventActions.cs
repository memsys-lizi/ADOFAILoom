using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class TextEventActions
    {
        private readonly DecorationMutationEngine decorations;
        private readonly VisualEventMutationEngine events;

        public TextEventActions(
            DecorationMutationEngine decorations,
            VisualEventMutationEngine events
        )
        {
            this.decorations = decorations;
            this.events = events;
        }

        public Task<VisualEventMutationResult> AddTextDecorationsAsync(
            string revision,
            AddTextEventCreate[] inputs,
            CancellationToken token
        ) => decorations.AddAsync(revision, inputs, CreateDecoration, token);

        public Task<VisualEventMutationResult> UpdateTextDecorationsAsync(
            string revision,
            AddTextEventUpdate[] inputs,
            CancellationToken token
        ) =>
            decorations.UpdateAsync(
                revision,
                inputs,
                LevelEventType.AddText,
                input => input.Reference,
                UpdateDecoration,
                token
            );

        public Task<VisualEventMutationResult> RemoveTextDecorationsAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => decorations.RemoveAsync(revision, references, LevelEventType.AddText, token);

        public Task<VisualEventMutationResult> AddSetTextAsync(
            string revision,
            SetTextEventCreate[] inputs,
            CancellationToken token
        ) => events.AddAsync(revision, inputs, CreateSetText, token);

        public Task<VisualEventMutationResult> UpdateSetTextAsync(
            string revision,
            SetTextEventUpdate[] inputs,
            CancellationToken token
        ) =>
            events.UpdateAsync(
                revision,
                inputs,
                LevelEventType.SetText,
                input => input.Reference,
                UpdateSetText,
                token
            );

        public Task<VisualEventMutationResult> RemoveSetTextAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => events.RemoveAsync(revision, references, LevelEventType.SetText, token);

        private static LevelEvent CreateDecoration(scnEditor editor, AddTextEventCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.AddText);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            VisualEventValidator.SetString(e, "decText", input.Text);
            VisualEventValidator.SetString(e, "tag", input.Tag);
            VisualEventValidator.SetEnum(e, "font", input.Font);
            VisualEventValidator.SetVector2(e, "position", input.Position);
            VisualEventValidator.SetEnum(e, "relativeTo", input.RelativeTo);
            VisualEventValidator.SetVector2(e, "pivotOffset", input.PivotOffset);
            VisualEventValidator.SetFloat(e, "rotation", input.Rotation);
            VisualEventValidator.SetBool(e, "lockRotation", input.LockRotation);
            VisualEventValidator.SetVector2(e, "scale", input.Scale);
            VisualEventValidator.SetBool(e, "lockScale", input.LockScale);
            VisualEventValidator.SetColor(e, "color", input.Color);
            VisualEventValidator.SetFloat(e, "opacity", input.Opacity);
            VisualEventValidator.SetInt(e, "depth", input.Depth);
            VisualEventValidator.SetVector2(e, "parallax", input.Parallax);
            VisualEventValidator.SetVector2(e, "parallaxOffset", input.ParallaxOffset);
            return e;
        }

        private static LevelEvent UpdateDecoration(
            scnEditor editor,
            LevelEvent e,
            AddTextEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            if (input.Text != null)
                VisualEventValidator.SetString(e, "decText", input.Text);
            if (input.Tag != null)
                VisualEventValidator.SetString(e, "tag", input.Tag);
            if (input.Font.HasValue)
                VisualEventValidator.SetEnum(e, "font", input.Font.Value);
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
            if (input.Color != null)
                VisualEventValidator.SetColor(e, "color", input.Color);
            if (input.Opacity.HasValue)
                VisualEventValidator.SetFloat(e, "opacity", input.Opacity.Value);
            if (input.Depth.HasValue)
                VisualEventValidator.SetInt(e, "depth", input.Depth.Value);
            if (input.Parallax != null)
                VisualEventValidator.SetVector2(e, "parallax", input.Parallax);
            if (input.ParallaxOffset != null)
                VisualEventValidator.SetVector2(e, "parallaxOffset", input.ParallaxOffset);
            return e;
        }

        private static LevelEvent CreateSetText(scnEditor editor, SetTextEventCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.SetText);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset);
            VisualEventValidator.SetString(e, "decText", input.Text);
            VisualEventValidator.SetString(e, "tag", input.Tag);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
            return e;
        }

        private static LevelEvent UpdateSetText(
            scnEditor editor,
            LevelEvent e,
            SetTextEventUpdate input
        )
        {
            ApplyFloor(editor, e, input);
            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset.Value);
            if (input.Text != null)
                VisualEventValidator.SetString(e, "decText", input.Text);
            if (input.Tag != null)
                VisualEventValidator.SetString(e, "tag", input.Tag);
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
    }
}
