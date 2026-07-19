using System;
using System.Linq;
using ADOFAI;
using UnityEngine;
using EventPropertyInfo = ADOFAI.PropertyInfo;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal static class VisualEventValidator
    {
        public static void ValidateFloor(
            scnEditor editor,
            LevelEventInfo eventInfo,
            int floor)
        {
            int lastFloor = editor.floors.Count - 1;
            if (floor < 0 || floor > lastFloor)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(floor),
                    $"Floor must be between 0 and {lastFloor}.");
            }

            if (floor == 0 && !eventInfo.allowFirstFloorCheck)
            {
                throw new ArgumentException(
                    $"{eventInfo.type} cannot be placed on the first floor.",
                    nameof(floor));
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
                    $"{levelEvent.eventType}.{key} must be between " +
                    $"{property.float_min} and {property.float_max}.");
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

        public static void SetVector2(
            LevelEvent levelEvent,
            string key,
            VisualVector2 value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(key);
            }

            EnsureFinite(key + ".x", value.X);
            EnsureFinite(key + ".y", value.Y);
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Vector2);
            if (!property.ignoreRange &&
                (value.X < property.minVec.x || value.X > property.maxVec.x ||
                 value.Y < property.minVec.y || value.Y > property.maxVec.y))
            {
                throw new ArgumentOutOfRangeException(
                    key,
                    $"{levelEvent.eventType}.{key} is outside the editor's allowed range.");
            }

            levelEvent[key] = new Vector2(value.X, value.Y);
            levelEvent.disabled[key] = false;
        }

        public static void SetEnum<TEnum>(LevelEvent levelEvent, string key, TEnum value)
            where TEnum : struct, Enum
        {
            EventPropertyInfo property = RequireProperty(levelEvent, key, PropertyType.Enum);
            Type enumType = property.enumType ??
                throw new InvalidOperationException(
                    $"{levelEvent.eventType}.{key} has no runtime enum type.");
            string name = value.ToString();
            if (!Enum.IsDefined(enumType, name))
            {
                throw new ArgumentException(
                    $"'{name}' is not supported by {levelEvent.eventType}.{key}.",
                    key);
            }

            object parsed = Enum.Parse(enumType, name, false);
            if (property.enumExceptions != null &&
                property.enumExceptions.Cast<object>().Any(exception => Equals(exception, parsed)))
            {
                throw new ArgumentException(
                    $"'{name}' is excluded by the editor for {levelEvent.eventType}.{key}.",
                    key);
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
                    $"{levelEvent.eventType}.{key} cannot be disabled.");
            }

            levelEvent.disabled[key] = true;
        }

        private static EventPropertyInfo RequireProperty(
            LevelEvent levelEvent,
            string key,
            PropertyType? expectedType = null)
        {
            if (!levelEvent.info.propertiesInfo.TryGetValue(key, out EventPropertyInfo property))
            {
                throw new InvalidOperationException(
                    $"The current game does not define {levelEvent.eventType}.{key}.");
            }

            if (expectedType.HasValue && property.type != expectedType.Value)
            {
                throw new InvalidOperationException(
                    $"The current game defines {levelEvent.eventType}.{key} as " +
                    $"{property.type}, not {expectedType.Value}.");
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
    }
}
