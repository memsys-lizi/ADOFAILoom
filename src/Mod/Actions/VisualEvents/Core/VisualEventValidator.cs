using System;
using System.Linq;
using ADOFAI;
using ADOFAI.Editor.Models;
using UnityEngine;
using EventPropertyInfo = ADOFAI.PropertyInfo;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal static class VisualEventValidator
    {
        public static void ValidateFloor(scnEditor editor, LevelEventInfo eventInfo, int floor)
        {
            int lastFloor = editor.floors.Count - 1;
            if (floor < 0 || floor > lastFloor)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(floor),
                    $"Floor must be between 0 and {lastFloor}."
                );
            }

            if (floor == 0 && !eventInfo.allowFirstFloorCheck)
            {
                throw new ArgumentException(
                    $"{eventInfo.type} cannot be placed on the first floor.",
                    nameof(floor)
                );
            }
        }

        public static void SetFloat(LevelEvent levelEvent, string key, float value)
        {
            EnsureFinite(key, value);
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Float);
            if (!property.ignoreRange && (value < property.float_min || value > property.float_max))
            {
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} must be between "
                        + $"{property.float_min} and {property.float_max}."
                );
            }

            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetBool(LevelEvent levelEvent, string key, bool value)
        {
            RequireProperty(levelEvent, key, PropertyType.Bool);
            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetInt(LevelEvent levelEvent, string key, int value)
        {
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Int);
            if (!property.ignoreRange && (value < property.int_min || value > property.int_max))
            {
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} must be between "
                        + $"{property.int_min} and {property.int_max}."
                );
            }

            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetString(LevelEvent levelEvent, string key, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(key);
            }

            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.String);
            if (
                value.Length < property.string_minLength
                || property.string_maxLength > 0 && value.Length > property.string_maxLength
            )
            {
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} length is outside the editor's allowed range."
                );
            }

            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetColor(LevelEvent levelEvent, string key, string value)
        {
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Color);
            int requiredLength = property.color_usesAlpha ? 8 : 6;
            if (value == null || value.Length != requiredLength || !value.All(IsHexDigit))
            {
                throw new ArgumentException(
                    $"{levelEvent.eventType}.{key} must contain exactly {requiredLength} hexadecimal characters.",
                    key
                );
            }

            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetFile(LevelEvent levelEvent, string key, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(key);
            }

            RequireProperty(levelEvent, key, PropertyType.File);
            levelEvent[key] = value;
            levelEvent.disabled[key] = false;
        }

        public static void SetVector2(LevelEvent levelEvent, string key, VisualVector2 value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(key);
            }

            EnsureFinite(key + ".x", value.X);
            EnsureFinite(key + ".y", value.Y);
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Vector2);
            if (
                !property.ignoreRange
                && (
                    value.X < property.minVec.x
                    || value.X > property.maxVec.x
                    || value.Y < property.minVec.y
                    || value.Y > property.maxVec.y
                )
            )
            {
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} is outside the editor's allowed range."
                );
            }

            levelEvent[key] = new Vector2(value.X, value.Y);
            levelEvent.disabled[key] = false;
        }

        public static void SetTile(LevelEvent levelEvent, string key, VisualTileReference value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(key);
            }

            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Tile);
            Type enumType = property.enumType ?? typeof(TileRelativeTo);
            string relativeTo = value.RelativeTo.ToString();
            if (!Enum.IsDefined(enumType, relativeTo))
            {
                throw new ArgumentException(
                    $"'{relativeTo}' is not supported by {levelEvent.eventType}.{key}.",
                    key
                );
            }

            object parsed = Enum.Parse(enumType, relativeTo, false);
            if (!(parsed is TileRelativeTo gameRelativeTo))
            {
                throw new InvalidOperationException(
                    $"The current game defines {levelEvent.eventType}.{key} with an incompatible tile enum."
                );
            }

            levelEvent[key] = new Tuple<int, TileRelativeTo>(value.Offset, gameRelativeTo);
            levelEvent.disabled[key] = false;
        }

        public static void SetFloatPair(LevelEvent levelEvent, string key, VisualFloatRange value)
        {
            if (value == null)
                throw new ArgumentNullException(key);
            EnsureFinite(key + ".minimum", value.Minimum);
            EnsureFinite(key + ".maximum", value.Maximum);
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.FloatPair);
            if (value.Minimum > value.Maximum)
                throw new ArgumentException(
                    $"{levelEvent.eventType}.{key} minimum cannot exceed maximum.",
                    key
                );
            if (value.Minimum < property.floatPairMin || value.Maximum > property.floatPairMax)
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} is outside the editor's allowed range."
                );
            levelEvent[key] = new Tuple<float, float>(value.Minimum, value.Maximum);
            levelEvent.disabled[key] = false;
        }

        public static void SetVector2Range(
            LevelEvent levelEvent,
            string key,
            VisualVector2Range value
        )
        {
            if (value == null)
                throw new ArgumentNullException(key);
            EnsureFinite(key + ".minimum.x", value.Minimum.X);
            EnsureFinite(key + ".minimum.y", value.Minimum.Y);
            EnsureFinite(key + ".maximum.x", value.Maximum.X);
            EnsureFinite(key + ".maximum.y", value.Maximum.Y);
            RequireProperty(levelEvent, key, PropertyType.Vector2Range);
            if (value.Minimum.X > value.Maximum.X || value.Minimum.Y > value.Maximum.Y)
                throw new ArgumentException(
                    $"{levelEvent.eventType}.{key} minimum components cannot exceed maximum components.",
                    key
                );
            levelEvent[key] = new Tuple<Vector2, Vector2>(
                new Vector2(value.Minimum.X, value.Minimum.Y),
                new Vector2(value.Maximum.X, value.Maximum.Y)
            );
            levelEvent.disabled[key] = false;
        }

        public static void SetMinMaxGradient(
            LevelEvent levelEvent,
            string key,
            VisualMinMaxGradient value
        )
        {
            RequireProperty(levelEvent, key, PropertyType.MinMaxGradient);
            SerializedMinMaxGradient converted = ParticleValueConverter.Convert(value);
            levelEvent[key] = converted;
            levelEvent.disabled[key] = false;
        }

        public static void SetEnum<TEnum>(LevelEvent levelEvent, string key, TEnum value)
            where TEnum : struct, Enum
        {
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Enum);
            Type enumType =
                property.enumType
                ?? throw new InvalidOperationException(
                    $"{levelEvent.eventType}.{key} has no runtime enum type."
                );
            string name = value.ToString();
            if (!Enum.IsDefined(enumType, name))
            {
                throw new ArgumentException(
                    $"'{name}' is not supported by {levelEvent.eventType}.{key}.",
                    key
                );
            }

            object parsed = Enum.Parse(enumType, name, false);
            if (
                property.enumExceptions != null
                && property
                    .enumExceptions.Cast<object>()
                    .Any(exception => Equals(exception, parsed))
            )
            {
                throw new ArgumentException(
                    $"'{name}' is excluded by the editor for {levelEvent.eventType}.{key}.",
                    key
                );
            }

            levelEvent[key] = parsed;
            levelEvent.disabled[key] = false;
        }

        public static void Disable(LevelEvent levelEvent, string key)
        {
            EventPropertyInfo property = RequireProperty(levelEvent, key);
            if (!property.canBeDisabled)
            {
                throw new InvalidOperationException(
                    $"{levelEvent.eventType}.{key} cannot be disabled."
                );
            }

            levelEvent.disabled[key] = true;
        }

        private static EventPropertyInfo RequireProperty(
            LevelEvent levelEvent,
            string key,
            PropertyType? expectedType = null
        )
        {
            if (!levelEvent.info.propertiesInfo.TryGetValue(key, out EventPropertyInfo property))
            {
                throw new InvalidOperationException(
                    $"The current game does not define {levelEvent.eventType}.{key}."
                );
            }

            if (expectedType.HasValue && property.type != expectedType.Value)
            {
                throw new InvalidOperationException(
                    $"The current game defines {levelEvent.eventType}.{key} as "
                        + $"{property.type}, not {expectedType.Value}."
                );
            }

            return property;
        }

        private static void EnsureFinite(string name, float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(name, "Numeric values must be finite.");
            }
        }

        private static bool IsHexDigit(char value)
        {
            return value >= '0' && value <= '9'
                || value >= 'a' && value <= 'f'
                || value >= 'A' && value <= 'F';
        }
    }
}
