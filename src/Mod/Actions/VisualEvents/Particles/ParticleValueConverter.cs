using System;
using System.Linq;
using ADOFAI.Editor.Models;
using UnityEngine;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal static class ParticleValueConverter
    {
        private const int MaximumGradientKeys = 8;

        public static SerializedMinMaxGradient Convert(VisualMinMaxGradient value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            ValidateShape(value);
            return new SerializedMinMaxGradient
            {
                mode = Enum.Parse<ParticleSystemGradientMode>(value.Mode.ToString()),
                color1 = value.Color1,
                color2 = value.Color2,
                gradient1 = value.Gradient1 == null ? null : Convert(value.Gradient1),
                gradient2 = value.Gradient2 == null ? null : Convert(value.Gradient2),
            };
        }

        private static SerializedGradient Convert(VisualGradient value)
        {
            ValidateKeys(value.ColorKeys, "colorKeys");
            ValidateKeys(value.AlphaKeys, "alphaKeys");
            return new SerializedGradient
            {
                mode = Enum.Parse<GradientMode>(value.Mode.ToString()),
                colorKeys = value
                    .ColorKeys.Select(key => new SerializedGradient.ColorKey
                    {
                        time = (decimal)key.Time,
                        color = ValidateColor(key.Color),
                    })
                    .ToArray(),
                alphaKeys = value
                    .AlphaKeys.Select(key => new SerializedGradient.AlphaKey
                    {
                        time = (decimal)key.Time,
                        alpha = (decimal)ValidateUnit(key.Alpha, "alpha"),
                    })
                    .ToArray(),
            };
        }

        private static void ValidateShape(VisualMinMaxGradient value)
        {
            bool c1 = value.Color1 != null,
                c2 = value.Color2 != null,
                g1 = value.Gradient1 != null,
                g2 = value.Gradient2 != null;
            bool valid = value.Mode switch
            {
                VisualParticleGradientMode.Color => c1 && !c2 && !g1 && !g2,
                VisualParticleGradientMode.TwoColors => c1 && c2 && !g1 && !g2,
                VisualParticleGradientMode.Gradient => !c1 && !c2 && g1 && !g2,
                VisualParticleGradientMode.RandomColor => !c1 && !c2 && g1 && !g2,
                VisualParticleGradientMode.TwoGradients => !c1 && !c2 && g1 && g2,
                _ => false,
            };
            if (!valid)
                throw new ArgumentException(
                    $"Gradient mode '{value.Mode}' has an invalid value combination."
                );
            if (c1)
                ValidateColor(value.Color1!);
            if (c2)
                ValidateColor(value.Color2!);
        }

        private static void ValidateKeys<T>(T[] values, string name)
        {
            if (values == null || values.Length < 2 || values.Length > MaximumGradientKeys)
                throw new ArgumentException(
                    $"{name} must contain between 2 and {MaximumGradientKeys} keys."
                );
            float previous = -1f;
            for (int index = 0; index < values.Length; index++)
            {
                float time = values[index] switch
                {
                    VisualGradientColorKey color => color.Time,
                    VisualGradientAlphaKey alpha => alpha.Time,
                    _ => throw new InvalidOperationException("Unsupported gradient key type."),
                };
                ValidateUnit(time, $"{name}[{index}].time");
                if (time <= previous)
                    throw new ArgumentException($"{name} times must be strictly increasing.");
                previous = time;
                if (values[index] is VisualGradientColorKey colorKey)
                    ValidateColor(colorKey.Color);
            }
        }

        private static string ValidateColor(string value)
        {
            if (value == null || value.Length != 6 || value.Any(character => !IsHex(character)))
                throw new ArgumentException(
                    "Particle gradient colors must contain exactly 6 hexadecimal characters."
                );
            return value;
        }

        private static float ValidateUnit(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 1f)
                throw new ArgumentOutOfRangeException(
                    name,
                    "Value must be finite and between 0 and 1."
                );
            return value;
        }

        private static bool IsHex(char value) =>
            value >= '0' && value <= '9'
            || value >= 'a' && value <= 'f'
            || value >= 'A' && value <= 'F';
    }
}
