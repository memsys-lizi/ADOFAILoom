using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualParticleSimulationSpace
    {
        Local,
        World,
    }

    internal enum VisualParticleShape
    {
        Rectangle,
        Circle,
    }

    internal enum VisualParticleArcMode
    {
        Loop,
        PingPong,
        BurstSpread,
        Random,
    }

    internal enum VisualParticlePlayMode
    {
        Start,
        Stop,
        Clear,
    }

    internal enum AddParticleProperty
    {
        ColorOverLifetime,
        SizeOverLifetime,
    }

    internal enum SetParticleProperty
    {
        TargetMode,
        Color,
        VelocityLimitOverLifetime,
        ColorOverLifetime,
        SizeOverLifetime,
        MaximumParticles,
        ParticleLifetime,
        ParticleSize,
        Velocity,
        RotationOverTime,
        ShapeType,
        ShapeRadius,
        Arc,
        ArcMode,
        EmissionRate,
        SimulationSpeed,
        LockRotation,
        LockScale,
    }

    internal sealed class ParticleDecorationCreate : FloorEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [JsonRequired]
        public string Image { get; set; } = string.Empty;

        [JsonRequired]
        public VisualVector2 RandomTextureTiling { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualParticleSimulationSpace SimulationSpace { get; set; }

        [JsonRequired]
        public VisualFloatRange StartRotation { get; set; } = new VisualFloatRange();

        [JsonRequired]
        public VisualMinMaxGradient Color { get; set; } = new VisualMinMaxGradient();

        [JsonRequired]
        public VisualFloatRange VelocityLimitOverLifetime { get; set; } = new VisualFloatRange();

        [McpOptional]
        public VisualMinMaxGradient? ColorOverLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? SizeOverLifetime { get; set; }

        [JsonRequired]
        public int MaximumParticles { get; set; }

        [JsonRequired]
        public bool AutoPlay { get; set; }

        [JsonRequired]
        public float PlayDuration { get; set; }

        [JsonRequired]
        public bool Loop { get; set; }

        [JsonRequired]
        public VisualFloatRange ParticleLifetime { get; set; } = new VisualFloatRange();

        [JsonRequired]
        public VisualFloatRange ParticleSize { get; set; } = new VisualFloatRange();

        [JsonRequired]
        public VisualVector2Range Velocity { get; set; } = new VisualVector2Range();

        [JsonRequired]
        public VisualFloatRange RotationOverTime { get; set; } = new VisualFloatRange();

        [JsonRequired]
        public VisualParticleShape ShapeType { get; set; }

        [JsonRequired]
        public float ShapeRadius { get; set; }

        [JsonRequired]
        public float Arc { get; set; }

        [JsonRequired]
        public VisualParticleArcMode ArcMode { get; set; }

        [JsonRequired]
        public VisualFloatRange EmissionRate { get; set; } = new VisualFloatRange();

        [JsonRequired]
        public float SimulationSpeed { get; set; }

        [JsonRequired]
        public int RandomSeed { get; set; }

        [JsonRequired]
        public VisualVector2 Position { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualDecorationPlacement RelativeTo { get; set; }

        [JsonRequired]
        public VisualVector2 PivotOffset { get; set; } = new VisualVector2();

        [JsonRequired]
        public float Rotation { get; set; }

        [JsonRequired]
        public VisualVector2 Scale { get; set; } = new VisualVector2();

        [JsonRequired]
        public int Depth { get; set; }

        [JsonRequired]
        public VisualVector2 Parallax { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualVector2 ParallaxOffset { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualMaskingType MaskingType { get; set; }

        [JsonRequired]
        public bool LockRotation { get; set; }

        [JsonRequired]
        public bool LockScale { get; set; }
    }

    internal sealed class ParticleDecorationUpdate : FloorEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public string? Image { get; set; }

        [McpOptional]
        public VisualVector2? RandomTextureTiling { get; set; }

        [McpOptional]
        public VisualParticleSimulationSpace? SimulationSpace { get; set; }

        [McpOptional]
        public VisualFloatRange? StartRotation { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? Color { get; set; }

        [McpOptional]
        public VisualFloatRange? VelocityLimitOverLifetime { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? ColorOverLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? SizeOverLifetime { get; set; }

        [McpOptional]
        public int? MaximumParticles { get; set; }

        [McpOptional]
        public bool? AutoPlay { get; set; }

        [McpOptional]
        public float? PlayDuration { get; set; }

        [McpOptional]
        public bool? Loop { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleSize { get; set; }

        [McpOptional]
        public VisualVector2Range? Velocity { get; set; }

        [McpOptional]
        public VisualFloatRange? RotationOverTime { get; set; }

        [McpOptional]
        public VisualParticleShape? ShapeType { get; set; }

        [McpOptional]
        public float? ShapeRadius { get; set; }

        [McpOptional]
        public float? Arc { get; set; }

        [McpOptional]
        public VisualParticleArcMode? ArcMode { get; set; }

        [McpOptional]
        public VisualFloatRange? EmissionRate { get; set; }

        [McpOptional]
        public float? SimulationSpeed { get; set; }

        [McpOptional]
        public int? RandomSeed { get; set; }

        [McpOptional]
        public VisualVector2? Position { get; set; }

        [McpOptional]
        public VisualDecorationPlacement? RelativeTo { get; set; }

        [McpOptional]
        public VisualVector2? PivotOffset { get; set; }

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }

        [McpOptional]
        public VisualMaskingType? MaskingType { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public bool? LockScale { get; set; }

        [McpOptional]
        public AddParticleProperty[]? DisabledProperties { get; set; }
    }

    internal sealed class SetParticleEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [McpOptional]
        public VisualParticlePlayMode? TargetMode { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? Color { get; set; }

        [McpOptional]
        public VisualFloatRange? VelocityLimitOverLifetime { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? ColorOverLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? SizeOverLifetime { get; set; }

        [McpOptional]
        public int? MaximumParticles { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleSize { get; set; }

        [McpOptional]
        public VisualVector2Range? Velocity { get; set; }

        [McpOptional]
        public VisualFloatRange? RotationOverTime { get; set; }

        [McpOptional]
        public VisualParticleShape? ShapeType { get; set; }

        [McpOptional]
        public float? ShapeRadius { get; set; }

        [McpOptional]
        public float? Arc { get; set; }

        [McpOptional]
        public VisualParticleArcMode? ArcMode { get; set; }

        [McpOptional]
        public int? EmissionRate { get; set; }

        [McpOptional]
        public float? SimulationSpeed { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public bool? LockScale { get; set; }
    }

    internal sealed class SetParticleEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public VisualParticlePlayMode? TargetMode { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? Color { get; set; }

        [McpOptional]
        public VisualFloatRange? VelocityLimitOverLifetime { get; set; }

        [McpOptional]
        public VisualMinMaxGradient? ColorOverLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? SizeOverLifetime { get; set; }

        [McpOptional]
        public int? MaximumParticles { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleLifetime { get; set; }

        [McpOptional]
        public VisualFloatRange? ParticleSize { get; set; }

        [McpOptional]
        public VisualVector2Range? Velocity { get; set; }

        [McpOptional]
        public VisualFloatRange? RotationOverTime { get; set; }

        [McpOptional]
        public VisualParticleShape? ShapeType { get; set; }

        [McpOptional]
        public float? ShapeRadius { get; set; }

        [McpOptional]
        public float? Arc { get; set; }

        [McpOptional]
        public VisualParticleArcMode? ArcMode { get; set; }

        [McpOptional]
        public int? EmissionRate { get; set; }

        [McpOptional]
        public float? SimulationSpeed { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public bool? LockScale { get; set; }

        [McpOptional]
        public SetParticleProperty[]? DisabledProperties { get; set; }
    }
}
