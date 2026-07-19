using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ADOFAILoom.Mcp.Protocol;

namespace ADOFAILoom.Mcp.Tooling
{
    internal sealed class McpToolRegistry
    {
        private readonly SortedDictionary<string, RegisteredTool> tools;

        private McpToolRegistry(SortedDictionary<string, RegisteredTool> tools)
        {
            this.tools = tools;
        }

        public IReadOnlyList<McpToolDefinition> Definitions =>
            tools.Values.Select(tool => tool.Definition).ToArray();

        public static McpToolRegistry Discover(
            Assembly assembly,
            IReadOnlyDictionary<Type, object> services,
            JsonSerializerOptions jsonOptions)
        {
            var discovered = new SortedDictionary<string, RegisteredTool>(StringComparer.Ordinal);
            var instances = new Dictionary<Type, object>();

            IEnumerable<MethodInfo> methods = assembly
                .GetTypes()
                .Where(type => !type.IsGenericTypeDefinition)
                .SelectMany(type => type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly))
                .Where(method => method.GetCustomAttribute<McpToolAttribute>() != null)
                .OrderBy(method => method.DeclaringType?.FullName, StringComparer.Ordinal)
                .ThenBy(method => method.Name, StringComparer.Ordinal);

            foreach (MethodInfo method in methods)
            {
                McpToolAttribute attribute = method.GetCustomAttribute<McpToolAttribute>()!;
                ValidateToolName(attribute.Name);
                if (string.IsNullOrWhiteSpace(attribute.Description))
                {
                    throw new InvalidOperationException(
                        $"MCP tool '{attribute.Name}' must declare a description.");
                }

                if (discovered.ContainsKey(attribute.Name))
                {
                    throw new InvalidOperationException(
                        $"Duplicate MCP tool name '{attribute.Name}'.");
                }

                object? target = null;
                if (!method.IsStatic)
                {
                    Type declaringType = method.DeclaringType ??
                        throw new InvalidOperationException(
                            $"MCP tool method '{method.Name}' has no declaring type.");
                    if (declaringType.IsAbstract)
                    {
                        throw new InvalidOperationException(
                            $"MCP tool container '{declaringType.FullName}' cannot be abstract.");
                    }

                    if (!instances.TryGetValue(declaringType, out target))
                    {
                        target = CreateInstance(declaringType, services);
                        instances.Add(declaringType, target);
                    }
                }

                JsonElement inputSchema = JsonSchemaGenerator.CreateInputSchema(method, jsonOptions);
                var definition = new McpToolDefinition(
                    attribute.Name,
                    attribute.Description,
                    inputSchema,
                    new McpToolAnnotations(
                        attribute.ReadOnly,
                        attribute.Destructive,
                        attribute.Idempotent,
                        attribute.OpenWorld));
                var invoker = new McpToolInvoker(target, method, jsonOptions);
                discovered.Add(attribute.Name, new RegisteredTool(definition, invoker));
            }

            if (discovered.Count == 0)
            {
                throw new InvalidOperationException("No MCP tools were discovered.");
            }

            return new McpToolRegistry(discovered);
        }

        public bool TryGet(string name, out McpToolInvoker? invoker)
        {
            if (tools.TryGetValue(name, out RegisteredTool? tool))
            {
                invoker = tool.Invoker;
                return true;
            }

            invoker = null;
            return false;
        }

        private static object CreateInstance(
            Type type,
            IReadOnlyDictionary<Type, object> services)
        {
            ConstructorInfo[] constructors = type.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException(
                    $"MCP tool container '{type.FullName}' must declare exactly one constructor.");
            }

            ParameterInfo[] parameters = constructors[0].GetParameters();
            var arguments = new object[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                Type serviceType = parameters[index].ParameterType;
                if (!services.TryGetValue(serviceType, out object? service))
                {
                    throw new InvalidOperationException(
                        $"MCP tool container '{type.FullName}' requires unregistered service " +
                        $"'{serviceType.FullName}'.");
                }

                arguments[index] = service;
            }

            try
            {
                return constructors[0].Invoke(arguments);
            }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"MCP tool container '{type.FullName}' could not be created.",
                    exception.InnerException);
            }
        }

        private static void ValidateToolName(string name)
        {
            if (name.Length == 0 || name.Length > 128)
            {
                throw new InvalidOperationException(
                    "MCP tool names must contain between 1 and 128 characters.");
            }

            foreach (char character in name)
            {
                bool valid = character >= 'a' && character <= 'z' ||
                             character >= 'A' && character <= 'Z' ||
                             character >= '0' && character <= '9' ||
                             character == '_' || character == '-' || character == '.';
                if (!valid)
                {
                    throw new InvalidOperationException(
                        $"MCP tool name '{name}' contains invalid character '{character}'.");
                }
            }
        }

        private sealed class RegisteredTool
        {
            public RegisteredTool(McpToolDefinition definition, McpToolInvoker invoker)
            {
                Definition = definition;
                Invoker = invoker;
            }

            public McpToolDefinition Definition { get; }

            public McpToolInvoker Invoker { get; }
        }
    }
}
