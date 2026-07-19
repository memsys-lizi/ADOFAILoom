using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class EmitParticleEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public EmitParticleEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            EmitParticleEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            EmitParticleEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.EmitParticle,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.EmitParticle, token);

        private static LevelEvent Create(scnEditor editor, EmitParticleEventCreate input)
        {
            var e = new LevelEvent(input.Floor, LevelEventType.EmitParticle);
            VisualEventValidator.ValidateFloor(editor, e.info, input.Floor);
            VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset);
            SetTag(e, input.Tag);
            VisualEventValidator.SetInt(e, "count", input.Count);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
            return e;
        }

        private static LevelEvent Update(
            scnEditor editor,
            LevelEvent e,
            EmitParticleEventUpdate input
        )
        {
            if (input.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, input.Floor.Value);
                e.floor = input.Floor.Value;
            }
            if (input.AngleOffset.HasValue)
                VisualEventValidator.SetFloat(e, "angleOffset", input.AngleOffset.Value);
            if (input.Tag != null)
                SetTag(e, input.Tag);
            if (input.Count.HasValue)
                VisualEventValidator.SetInt(e, "count", input.Count.Value);
            if (input.EventTag != null)
                VisualEventValidator.SetString(e, "eventTag", input.EventTag);
            return e;
        }

        private static void SetTag(LevelEvent e, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Particle tag cannot be empty or whitespace.");
            VisualEventValidator.SetString(e, "tag", tag);
        }
    }
}
