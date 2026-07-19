using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class ParticleDecorationActions
    {
        private readonly DecorationMutationEngine engine;

        public ParticleDecorationActions(DecorationMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            ParticleDecorationCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            ParticleDecorationUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.AddParticle,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.AddParticle, token);

        private static LevelEvent Create(scnEditor editor, ParticleDecorationCreate i)
        {
            var e = new LevelEvent(i.Floor, LevelEventType.AddParticle);
            VisualEventValidator.ValidateFloor(editor, e.info, i.Floor);
            SetTag(e, i.Tag);
            SetImage(e, i.Image);
            VisualEventValidator.SetVector2(e, "randomTextureTiling", i.RandomTextureTiling);
            VisualEventValidator.SetEnum(e, "simulationSpace", i.SimulationSpace);
            VisualEventValidator.SetFloatPair(e, "startRotation", i.StartRotation);
            VisualEventValidator.SetMinMaxGradient(e, "color", i.Color);
            VisualEventValidator.SetFloatPair(
                e,
                "velocityLimitOverLifetime",
                i.VelocityLimitOverLifetime
            );
            SetOptionalGradient(e, "colorOverLifetime", i.ColorOverLifetime);
            SetOptionalPair(e, "sizeOverLifetime", i.SizeOverLifetime);
            VisualEventValidator.SetInt(e, "maxParticles", i.MaximumParticles);
            VisualEventValidator.SetBool(e, "autoPlay", i.AutoPlay);
            VisualEventValidator.SetFloat(e, "playDuration", i.PlayDuration);
            VisualEventValidator.SetBool(e, "loop", i.Loop);
            VisualEventValidator.SetFloatPair(e, "particleLifetime", i.ParticleLifetime);
            VisualEventValidator.SetFloatPair(e, "particleSize", i.ParticleSize);
            VisualEventValidator.SetVector2Range(e, "velocity", i.Velocity);
            VisualEventValidator.SetFloatPair(e, "rotationOverTime", i.RotationOverTime);
            VisualEventValidator.SetEnum(e, "shapeType", i.ShapeType);
            VisualEventValidator.SetFloat(e, "shapeRadius", i.ShapeRadius);
            VisualEventValidator.SetFloat(e, "arc", i.Arc);
            VisualEventValidator.SetEnum(e, "arcMode", i.ArcMode);
            VisualEventValidator.SetFloatPair(e, "emissionRate", i.EmissionRate);
            VisualEventValidator.SetFloat(e, "simulationSpeed", i.SimulationSpeed);
            VisualEventValidator.SetInt(e, "randomSeed", i.RandomSeed);
            SetPlacement(e, i);
            return e;
        }

        private static LevelEvent Update(scnEditor editor, LevelEvent e, ParticleDecorationUpdate i)
        {
            if (i.Floor.HasValue)
            {
                VisualEventValidator.ValidateFloor(editor, e.info, i.Floor.Value);
                e.floor = i.Floor.Value;
            }
            var disabled = new DisabledPropertySet<AddParticleProperty>(i.DisabledProperties);
            disabled.Apply(
                e,
                AddParticleProperty.ColorOverLifetime,
                i.ColorOverLifetime != null,
                "colorOverLifetime"
            );
            disabled.Apply(
                e,
                AddParticleProperty.SizeOverLifetime,
                i.SizeOverLifetime != null,
                "sizeOverLifetime"
            );
            if (i.Tag != null)
                SetTag(e, i.Tag);
            if (i.Image != null)
                SetImage(e, i.Image);
            if (i.RandomTextureTiling != null)
                VisualEventValidator.SetVector2(e, "randomTextureTiling", i.RandomTextureTiling);
            if (i.SimulationSpace.HasValue)
                VisualEventValidator.SetEnum(e, "simulationSpace", i.SimulationSpace.Value);
            if (i.StartRotation != null)
                VisualEventValidator.SetFloatPair(e, "startRotation", i.StartRotation);
            if (i.Color != null)
                VisualEventValidator.SetMinMaxGradient(e, "color", i.Color);
            if (i.VelocityLimitOverLifetime != null)
                VisualEventValidator.SetFloatPair(
                    e,
                    "velocityLimitOverLifetime",
                    i.VelocityLimitOverLifetime
                );
            if (i.ColorOverLifetime != null)
                VisualEventValidator.SetMinMaxGradient(e, "colorOverLifetime", i.ColorOverLifetime);
            if (i.SizeOverLifetime != null)
                VisualEventValidator.SetFloatPair(e, "sizeOverLifetime", i.SizeOverLifetime);
            if (i.MaximumParticles.HasValue)
                VisualEventValidator.SetInt(e, "maxParticles", i.MaximumParticles.Value);
            if (i.AutoPlay.HasValue)
                VisualEventValidator.SetBool(e, "autoPlay", i.AutoPlay.Value);
            if (i.PlayDuration.HasValue)
                VisualEventValidator.SetFloat(e, "playDuration", i.PlayDuration.Value);
            if (i.Loop.HasValue)
                VisualEventValidator.SetBool(e, "loop", i.Loop.Value);
            if (i.ParticleLifetime != null)
                VisualEventValidator.SetFloatPair(e, "particleLifetime", i.ParticleLifetime);
            if (i.ParticleSize != null)
                VisualEventValidator.SetFloatPair(e, "particleSize", i.ParticleSize);
            if (i.Velocity != null)
                VisualEventValidator.SetVector2Range(e, "velocity", i.Velocity);
            if (i.RotationOverTime != null)
                VisualEventValidator.SetFloatPair(e, "rotationOverTime", i.RotationOverTime);
            if (i.ShapeType.HasValue)
                VisualEventValidator.SetEnum(e, "shapeType", i.ShapeType.Value);
            if (i.ShapeRadius.HasValue)
                VisualEventValidator.SetFloat(e, "shapeRadius", i.ShapeRadius.Value);
            if (i.Arc.HasValue)
                VisualEventValidator.SetFloat(e, "arc", i.Arc.Value);
            if (i.ArcMode.HasValue)
                VisualEventValidator.SetEnum(e, "arcMode", i.ArcMode.Value);
            if (i.EmissionRate != null)
                VisualEventValidator.SetFloatPair(e, "emissionRate", i.EmissionRate);
            if (i.SimulationSpeed.HasValue)
                VisualEventValidator.SetFloat(e, "simulationSpeed", i.SimulationSpeed.Value);
            if (i.RandomSeed.HasValue)
                VisualEventValidator.SetInt(e, "randomSeed", i.RandomSeed.Value);
            ApplyPlacement(e, i);
            return e;
        }

        private static void SetPlacement(LevelEvent e, ParticleDecorationCreate i)
        {
            VisualEventValidator.SetVector2(e, "position", i.Position);
            VisualEventValidator.SetEnum(e, "relativeTo", i.RelativeTo);
            VisualEventValidator.SetVector2(e, "pivotOffset", i.PivotOffset);
            VisualEventValidator.SetFloat(e, "rotation", i.Rotation);
            VisualEventValidator.SetVector2(e, "scale", i.Scale);
            VisualEventValidator.SetInt(e, "depth", i.Depth);
            VisualEventValidator.SetVector2(e, "parallax", i.Parallax);
            VisualEventValidator.SetVector2(e, "parallaxOffset", i.ParallaxOffset);
            VisualEventValidator.SetEnum(e, "maskingType", i.MaskingType);
            VisualEventValidator.SetBool(e, "lockRotation", i.LockRotation);
            VisualEventValidator.SetBool(e, "lockScale", i.LockScale);
        }

        private static void ApplyPlacement(LevelEvent e, ParticleDecorationUpdate i)
        {
            if (i.Position != null)
                VisualEventValidator.SetVector2(e, "position", i.Position);
            if (i.RelativeTo.HasValue)
                VisualEventValidator.SetEnum(e, "relativeTo", i.RelativeTo.Value);
            if (i.PivotOffset != null)
                VisualEventValidator.SetVector2(e, "pivotOffset", i.PivotOffset);
            if (i.Rotation.HasValue)
                VisualEventValidator.SetFloat(e, "rotation", i.Rotation.Value);
            if (i.Scale != null)
                VisualEventValidator.SetVector2(e, "scale", i.Scale);
            if (i.Depth.HasValue)
                VisualEventValidator.SetInt(e, "depth", i.Depth.Value);
            if (i.Parallax != null)
                VisualEventValidator.SetVector2(e, "parallax", i.Parallax);
            if (i.ParallaxOffset != null)
                VisualEventValidator.SetVector2(e, "parallaxOffset", i.ParallaxOffset);
            if (i.MaskingType.HasValue)
                VisualEventValidator.SetEnum(e, "maskingType", i.MaskingType.Value);
            if (i.LockRotation.HasValue)
                VisualEventValidator.SetBool(e, "lockRotation", i.LockRotation.Value);
            if (i.LockScale.HasValue)
                VisualEventValidator.SetBool(e, "lockScale", i.LockScale.Value);
        }

        private static void SetTag(LevelEvent e, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException(
                    "Particle decoration tag cannot be empty or whitespace."
                );
            VisualEventValidator.SetString(e, "tag", tag);
        }

        private static void SetImage(LevelEvent e, string image)
        {
            EditorAssetValidator.ValidateExistingLevelAsset(image);
            VisualEventValidator.SetFile(e, "decorationImage", image);
        }

        private static void SetOptionalGradient(
            LevelEvent e,
            string key,
            VisualMinMaxGradient? value
        )
        {
            if (value == null)
                VisualEventValidator.Disable(e, key);
            else
                VisualEventValidator.SetMinMaxGradient(e, key, value);
        }

        private static void SetOptionalPair(LevelEvent e, string key, VisualFloatRange? value)
        {
            if (value == null)
                VisualEventValidator.Disable(e, key);
            else
                VisualEventValidator.SetFloatPair(e, key, value);
        }
    }
}
