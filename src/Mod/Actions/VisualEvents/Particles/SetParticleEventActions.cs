using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class SetParticleEventActions
    {
        private readonly VisualEventMutationEngine engine;

        public SetParticleEventActions(VisualEventMutationEngine engine) => this.engine = engine;

        public Task<VisualEventMutationResult> AddAsync(
            string revision,
            SetParticleEventCreate[] events,
            CancellationToken token
        ) => engine.AddAsync(revision, events, Create, token);

        public Task<VisualEventMutationResult> UpdateAsync(
            string revision,
            SetParticleEventUpdate[] events,
            CancellationToken token
        ) =>
            engine.UpdateAsync(
                revision,
                events,
                LevelEventType.SetParticle,
                input => input.Reference,
                Update,
                token
            );

        public Task<VisualEventMutationResult> RemoveAsync(
            string revision,
            VisualEventReference[] references,
            CancellationToken token
        ) => engine.RemoveAsync(revision, references, LevelEventType.SetParticle, token);

        private static LevelEvent Create(scnEditor editor, SetParticleEventCreate i)
        {
            var e = new LevelEvent(i.Floor, LevelEventType.SetParticle);
            VisualEventValidator.ValidateFloor(editor, e.info, i.Floor);
            SetTween(e, i.AngleOffset, i.Duration, i.Ease, i.EventTag);
            SetTag(e, i.Tag);
            int count =
                SetOptional(e, "targetMode", i.TargetMode)
                + SetOptional(e, "color", i.Color)
                + SetOptionalPair(e, "velocityLimitOverLifetime", i.VelocityLimitOverLifetime)
                + SetOptional(e, "colorOverLifetime", i.ColorOverLifetime)
                + SetOptionalPair(e, "sizeOverLifetime", i.SizeOverLifetime)
                + SetOptionalInt(e, "maxParticles", i.MaximumParticles)
                + SetOptionalPair(e, "particleLifetime", i.ParticleLifetime)
                + SetOptionalPair(e, "particleSize", i.ParticleSize)
                + SetOptional(e, "velocity", i.Velocity)
                + SetOptionalPair(e, "rotationOverTime", i.RotationOverTime)
                + SetOptional(e, "shapeType", i.ShapeType)
                + SetOptionalFloat(e, "shapeRadius", i.ShapeRadius)
                + SetOptionalFloat(e, "arc", i.Arc)
                + SetOptional(e, "arcMode", i.ArcMode)
                + SetOptionalInt(e, "emissionRate", i.EmissionRate)
                + SetOptionalFloat(e, "simulationSpeed", i.SimulationSpeed)
                + SetOptionalBool(e, "lockRotation", i.LockRotation)
                + SetOptionalBool(e, "lockScale", i.LockScale);
            if (count == 0)
                throw new ArgumentException(
                    "SetParticle must enable at least one particle property."
                );
            return e;
        }

        private static LevelEvent Update(scnEditor editor, LevelEvent e, SetParticleEventUpdate i)
        {
            ApplyTween(editor, e, i);
            if (i.Tag != null)
                SetTag(e, i.Tag);
            var disabled = new DisabledPropertySet<SetParticleProperty>(i.DisabledProperties);
            ApplyDisabled(disabled, e, i);
            if (i.TargetMode.HasValue)
                VisualEventValidator.SetEnum(e, "targetMode", i.TargetMode.Value);
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
            if (i.EmissionRate.HasValue)
                VisualEventValidator.SetInt(e, "emissionRate", i.EmissionRate.Value);
            if (i.SimulationSpeed.HasValue)
                VisualEventValidator.SetFloat(e, "simulationSpeed", i.SimulationSpeed.Value);
            if (i.LockRotation.HasValue)
                VisualEventValidator.SetBool(e, "lockRotation", i.LockRotation.Value);
            if (i.LockScale.HasValue)
                VisualEventValidator.SetBool(e, "lockScale", i.LockScale.Value);
            return e;
        }

        private static void ApplyDisabled(
            DisabledPropertySet<SetParticleProperty> d,
            LevelEvent e,
            SetParticleEventUpdate i
        )
        {
            d.Apply(e, SetParticleProperty.TargetMode, i.TargetMode.HasValue, "targetMode");
            d.Apply(e, SetParticleProperty.Color, i.Color != null, "color");
            d.Apply(
                e,
                SetParticleProperty.VelocityLimitOverLifetime,
                i.VelocityLimitOverLifetime != null,
                "velocityLimitOverLifetime"
            );
            d.Apply(
                e,
                SetParticleProperty.ColorOverLifetime,
                i.ColorOverLifetime != null,
                "colorOverLifetime"
            );
            d.Apply(
                e,
                SetParticleProperty.SizeOverLifetime,
                i.SizeOverLifetime != null,
                "sizeOverLifetime"
            );
            d.Apply(
                e,
                SetParticleProperty.MaximumParticles,
                i.MaximumParticles.HasValue,
                "maxParticles"
            );
            d.Apply(
                e,
                SetParticleProperty.ParticleLifetime,
                i.ParticleLifetime != null,
                "particleLifetime"
            );
            d.Apply(e, SetParticleProperty.ParticleSize, i.ParticleSize != null, "particleSize");
            d.Apply(e, SetParticleProperty.Velocity, i.Velocity != null, "velocity");
            d.Apply(
                e,
                SetParticleProperty.RotationOverTime,
                i.RotationOverTime != null,
                "rotationOverTime"
            );
            d.Apply(e, SetParticleProperty.ShapeType, i.ShapeType.HasValue, "shapeType");
            d.Apply(e, SetParticleProperty.ShapeRadius, i.ShapeRadius.HasValue, "shapeRadius");
            d.Apply(e, SetParticleProperty.Arc, i.Arc.HasValue, "arc");
            d.Apply(e, SetParticleProperty.ArcMode, i.ArcMode.HasValue, "arcMode");
            d.Apply(e, SetParticleProperty.EmissionRate, i.EmissionRate.HasValue, "emissionRate");
            d.Apply(
                e,
                SetParticleProperty.SimulationSpeed,
                i.SimulationSpeed.HasValue,
                "simulationSpeed"
            );
            d.Apply(e, SetParticleProperty.LockRotation, i.LockRotation.HasValue, "lockRotation");
            d.Apply(e, SetParticleProperty.LockScale, i.LockScale.HasValue, "lockScale");
        }

        private static void SetTween(
            LevelEvent e,
            float angle,
            float duration,
            VisualEase ease,
            string? eventTag
        )
        {
            VisualEventValidator.SetFloat(e, "angleOffset", angle);
            VisualEventValidator.SetFloat(e, "duration", duration);
            VisualEventValidator.SetEnum(e, "ease", ease);
            if (eventTag != null)
                VisualEventValidator.SetString(e, "eventTag", eventTag);
        }

        private static void ApplyTween(scnEditor editor, LevelEvent e, SetParticleEventUpdate i)
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
                throw new ArgumentException("SetParticle tag cannot be empty or whitespace.");
            VisualEventValidator.SetString(e, "tag", tag);
        }

        private static int SetOptional(LevelEvent e, string key, VisualMinMaxGradient? v)
        {
            if (v == null)
                return Disable(e, key);
            VisualEventValidator.SetMinMaxGradient(e, key, v);
            return 1;
        }

        private static int SetOptional(LevelEvent e, string key, VisualVector2Range? v)
        {
            if (v == null)
                return Disable(e, key);
            VisualEventValidator.SetVector2Range(e, key, v);
            return 1;
        }

        private static int SetOptionalPair(LevelEvent e, string key, VisualFloatRange? v)
        {
            if (v == null)
                return Disable(e, key);
            VisualEventValidator.SetFloatPair(e, key, v);
            return 1;
        }

        private static int SetOptionalInt(LevelEvent e, string key, int? v)
        {
            if (!v.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetInt(e, key, v.Value);
            return 1;
        }

        private static int SetOptionalFloat(LevelEvent e, string key, float? v)
        {
            if (!v.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetFloat(e, key, v.Value);
            return 1;
        }

        private static int SetOptionalBool(LevelEvent e, string key, bool? v)
        {
            if (!v.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetBool(e, key, v.Value);
            return 1;
        }

        private static int SetOptional<T>(LevelEvent e, string key, T? v)
            where T : struct, Enum
        {
            if (!v.HasValue)
                return Disable(e, key);
            VisualEventValidator.SetEnum(e, key, v.Value);
            return 1;
        }

        private static int Disable(LevelEvent e, string key)
        {
            VisualEventValidator.Disable(e, key);
            return 0;
        }
    }
}
