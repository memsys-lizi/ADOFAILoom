using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADOFAILoom.Mcp.Tooling
{
    internal static class JsonSchemaGenerator
    {
        public static JsonElement CreateInputSchema(
            MethodInfo method,
            JsonSerializerOptions options
        )
        {
            var properties = new SortedDictionary<string, object?>(StringComparer.Ordinal);
            var required = new List<string>();

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof(System.Threading.CancellationToken))
                {
                    continue;
                }

                string name = GetJsonName(parameter.Name);
                object schema = CreateTypeSchema(parameter.ParameterType, new HashSet<Type>());
                string? description = parameter
                    .GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
                if (!string.IsNullOrWhiteSpace(description))
                {
                    ((IDictionary<string, object?>)schema)["description"] = description;
                }

                properties.Add(name, schema);

                bool optional =
                    parameter.HasDefaultValue
                    || parameter.GetCustomAttribute<McpOptionalAttribute>() != null;
                if (!optional)
                {
                    required.Add(name);
                }
            }

            var root = new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = properties,
                ["additionalProperties"] = false,
            };
            if (required.Count > 0)
            {
                root["required"] = required;
            }

            return JsonSerializer.SerializeToElement(root, options);
        }

        private static IDictionary<string, object?> CreateTypeSchema(Type type, ISet<Type> visiting)
        {
            Type? nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                // Nullable<T> represents an omittable input in the reflection contract.
                // Explicit JSON null is never a valid tool argument.
                return CreateTypeSchema(nullableType, visiting);
            }

            if (type == typeof(string) || type == typeof(char))
            {
                return TypeSchema("string");
            }

            if (type == typeof(bool))
            {
                return TypeSchema("boolean");
            }

            if (IsInteger(type))
            {
                return TypeSchema("integer");
            }

            if (IsNumber(type))
            {
                return TypeSchema("number");
            }

            if (type.IsEnum)
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = "string",
                    ["enum"] = Enum.GetNames(type),
                };
            }

            Type? elementType = GetCollectionElementType(type);
            if (elementType != null)
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = CreateTypeSchema(elementType, visiting),
                };
            }

            if (type == typeof(object) || type == typeof(JsonElement) || type.Namespace == "System")
            {
                throw new InvalidOperationException(
                    $"MCP input type '{type.FullName}' does not have a strict JSON schema mapping."
                );
            }

            if (!visiting.Add(type))
            {
                throw new InvalidOperationException(
                    $"Recursive MCP input type '{type.FullName}' is not supported."
                );
            }

            try
            {
                var properties = new SortedDictionary<string, object?>(StringComparer.Ordinal);
                var required = new List<string>();
                PropertyInfo[] serializableProperties = type.GetProperties(
                        BindingFlags.Instance | BindingFlags.Public
                    )
                    .Where(property => property.GetMethod != null && property.SetMethod != null)
                    .Where(property => property.GetIndexParameters().Length == 0)
                    .Where(property => property.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                    .ToArray();

                if (serializableProperties.Length == 0)
                {
                    throw new InvalidOperationException(
                        $"MCP input DTO '{type.FullName}' has no public readable and writable properties."
                    );
                }

                foreach (PropertyInfo property in serializableProperties)
                {
                    string name =
                        property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                        ?? JsonNamingPolicy.CamelCase.ConvertName(property.Name);
                    IDictionary<string, object?> schema = CreateTypeSchema(
                        property.PropertyType,
                        visiting
                    );
                    string? description = property
                        .GetCustomAttribute<DescriptionAttribute>()
                        ?.Description;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        schema["description"] = description;
                    }

                    properties.Add(name, schema);
                    bool optional = property.GetCustomAttribute<McpOptionalAttribute>() != null;
                    if (!optional)
                    {
                        required.Add(name);
                    }
                }

                var result = new Dictionary<string, object?>
                {
                    ["type"] = "object",
                    ["properties"] = properties,
                    ["additionalProperties"] = false,
                };
                if (required.Count > 0)
                {
                    result["required"] = required;
                }

                return result;
            }
            finally
            {
                visiting.Remove(type);
            }
        }

        private static Dictionary<string, object?> TypeSchema(string type)
        {
            return new Dictionary<string, object?> { ["type"] = type };
        }

        private static string GetJsonName(string? parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new InvalidOperationException("MCP tool parameters must have names.");
            }

            return JsonNamingPolicy.CamelCase.ConvertName(parameterName);
        }

        private static bool IsInteger(Type type)
        {
            return type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong);
        }

        private static bool IsNumber(Type type)
        {
            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        private static Type? GetCollectionElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (!type.IsGenericType || type == typeof(string))
            {
                return null;
            }

            Type genericDefinition = type.GetGenericTypeDefinition();
            if (
                genericDefinition == typeof(List<>)
                || genericDefinition == typeof(IReadOnlyList<>)
                || genericDefinition == typeof(ICollection<>)
                || genericDefinition == typeof(IEnumerable<>)
            )
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
