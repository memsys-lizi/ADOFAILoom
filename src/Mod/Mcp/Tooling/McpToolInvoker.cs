using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ADOFAILoom.Mcp.Tooling
{
    internal sealed class McpToolInvoker
    {
        private readonly object? target;
        private readonly MethodInfo method;
        private readonly ParameterInfo[] parameters;
        private readonly Type resultType;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly HashSet<string> argumentNames;

        public McpToolInvoker(
            object? target,
            MethodInfo method,
            JsonSerializerOptions jsonOptions)
        {
            this.target = target;
            this.method = method;
            this.jsonOptions = jsonOptions;
            parameters = method.GetParameters();
            resultType = ValidateSignature(method, parameters);
            argumentNames = parameters
                .Where(parameter => parameter.ParameterType != typeof(CancellationToken))
                .Select(parameter => JsonNamingPolicy.CamelCase.ConvertName(parameter.Name!))
                .ToHashSet(StringComparer.Ordinal);
        }

        public async Task<JsonElement> InvokeAsync(
            JsonElement? arguments,
            CancellationToken cancellationToken)
        {
            JsonElement argumentObject = arguments ?? EmptyArguments();
            if (argumentObject.ValueKind != JsonValueKind.Object)
            {
                throw new McpInvalidArgumentsException("Tool arguments must be a JSON object.");
            }

            RejectExplicitNulls(argumentObject, string.Empty);
            foreach (JsonProperty property in argumentObject.EnumerateObject())
            {
                if (!argumentNames.Contains(property.Name))
                {
                    throw new McpInvalidArgumentsException(
                        $"Unknown tool argument '{property.Name}'.");
                }
            }

            var invocationArguments = new object?[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                ParameterInfo parameter = parameters[index];
                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    invocationArguments[index] = cancellationToken;
                    continue;
                }

                string name = JsonNamingPolicy.CamelCase.ConvertName(parameter.Name!);
                if (!argumentObject.TryGetProperty(name, out JsonElement value))
                {
                    if (parameter.HasDefaultValue)
                    {
                        invocationArguments[index] = parameter.DefaultValue;
                        continue;
                    }

                    if (parameter.GetCustomAttribute<McpOptionalAttribute>() != null)
                    {
                        invocationArguments[index] = null;
                        continue;
                    }

                    throw new McpInvalidArgumentsException(
                        $"Required tool argument '{name}' is missing.");
                }

                try
                {
                    object? deserialized = JsonSerializer.Deserialize(
                        value.GetRawText(),
                        parameter.ParameterType,
                        jsonOptions);
                    if (deserialized == null && Nullable.GetUnderlyingType(parameter.ParameterType) == null)
                    {
                        throw new McpInvalidArgumentsException(
                            $"Tool argument '{name}' cannot be null.");
                    }

                    invocationArguments[index] = deserialized;
                }
                catch (JsonException exception)
                {
                    throw new McpInvalidArgumentsException(
                        $"Tool argument '{name}' is invalid: {exception.Message}",
                        exception);
                }
            }

            object? invocationResult;
            try
            {
                invocationResult = method.Invoke(target, invocationArguments);
            }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }

            object? result;
            if (invocationResult is Task task)
            {
                await task.ConfigureAwait(false);
                result = task.GetType().GetProperty("Result")!.GetValue(task);
            }
            else
            {
                result = invocationResult;
            }

            return JsonSerializer.SerializeToElement(result, resultType, jsonOptions);
        }

        private static Type ValidateSignature(MethodInfo method, IEnumerable<ParameterInfo> parameters)
        {
            if (method.IsGenericMethodDefinition || method.ContainsGenericParameters)
            {
                throw new InvalidOperationException(
                    $"MCP tool method '{method.DeclaringType?.FullName}.{method.Name}' cannot be generic.");
            }

            int cancellationTokens = 0;
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType.IsByRef || parameter.IsOut)
                {
                    throw new InvalidOperationException(
                        $"MCP tool method '{method.Name}' cannot use ref or out parameters.");
                }

                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    cancellationTokens++;
                    continue;
                }

                if (parameter.GetCustomAttribute<McpOptionalAttribute>() != null &&
                    !parameter.HasDefaultValue &&
                    parameter.ParameterType.IsValueType &&
                    Nullable.GetUnderlyingType(parameter.ParameterType) == null)
                {
                    throw new InvalidOperationException(
                        $"Optional MCP tool parameter '{parameter.Name}' on method '{method.Name}' " +
                        "must be nullable or declare a default value.");
                }

                if (string.IsNullOrWhiteSpace(parameter.Name))
                {
                    throw new InvalidOperationException(
                        $"MCP tool method '{method.Name}' contains an unnamed parameter.");
                }

                string jsonName = JsonNamingPolicy.CamelCase.ConvertName(parameter.Name);
                if (!names.Add(jsonName))
                {
                    throw new InvalidOperationException(
                        $"MCP tool method '{method.Name}' contains duplicate JSON parameter '{jsonName}'.");
                }
            }

            if (cancellationTokens > 1)
            {
                throw new InvalidOperationException(
                    $"MCP tool method '{method.Name}' contains more than one CancellationToken.");
            }

            Type returnType = method.ReturnType;
            if (returnType == typeof(void) || returnType == typeof(Task))
            {
                throw new InvalidOperationException(
                    $"MCP tool method '{method.Name}' must return a value or Task<T>.");
            }

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(Task<>))
                {
                    throw new InvalidOperationException(
                        $"MCP tool method '{method.Name}' must return Task<T>, not '{returnType.FullName}'.");
                }

                return returnType.GetGenericArguments()[0];
            }

            return returnType;
        }

        private static JsonElement EmptyArguments()
        {
            return JsonSerializer.SerializeToElement(new Dictionary<string, object?>());
        }

        private static void RejectExplicitNulls(JsonElement element, string path)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    string propertyPath = string.IsNullOrEmpty(path)
                        ? property.Name
                        : path + "." + property.Name;
                    RejectExplicitNulls(property.Value, propertyPath);
                }

                return;
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (JsonElement item in element.EnumerateArray())
                {
                    RejectExplicitNulls(item, $"{path}[{index}]");
                    index++;
                }

                return;
            }

            if (element.ValueKind == JsonValueKind.Null)
            {
                throw new McpInvalidArgumentsException(
                    $"Tool argument '{path}' cannot be null. Omit optional values instead.");
            }
        }
    }

    internal sealed class McpInvalidArgumentsException : Exception
    {
        public McpInvalidArgumentsException(string message)
            : base(message)
        {
        }

        public McpInvalidArgumentsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
