using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOFAILoom.Mcp.Tooling
{
    internal sealed class McpToolAvailability
    {
        private readonly object gate = new object();
        private readonly HashSet<string> knownTools;
        private readonly HashSet<string> disabledTools;

        public McpToolAvailability(
            IEnumerable<string> knownToolNames,
            IEnumerable<string> disabledToolNames
        )
        {
            if (knownToolNames == null)
            {
                throw new ArgumentNullException(nameof(knownToolNames));
            }

            if (disabledToolNames == null)
            {
                throw new ArgumentNullException(nameof(disabledToolNames));
            }

            knownTools = new HashSet<string>(knownToolNames, StringComparer.Ordinal);
            if (knownTools.Count == 0)
            {
                throw new InvalidOperationException("The MCP tool catalog cannot be empty.");
            }

            disabledTools = new HashSet<string>(StringComparer.Ordinal);
            foreach (string toolName in disabledToolNames)
            {
                if (string.IsNullOrEmpty(toolName) || !knownTools.Contains(toolName))
                {
                    throw new InvalidOperationException(
                        $"The Mod settings contain unknown disabled tool '{toolName}'."
                    );
                }

                if (!disabledTools.Add(toolName))
                {
                    throw new InvalidOperationException(
                        $"The Mod settings contain duplicate disabled tool '{toolName}'."
                    );
                }
            }
        }

        public bool IsEnabled(string toolName)
        {
            RequireKnown(toolName);
            lock (gate)
            {
                return !disabledTools.Contains(toolName);
            }
        }

        public void SetEnabled(string toolName, bool enabled)
        {
            RequireKnown(toolName);
            lock (gate)
            {
                if (enabled)
                {
                    disabledTools.Remove(toolName);
                }
                else
                {
                    disabledTools.Add(toolName);
                }
            }
        }

        public void SetEnabled(IEnumerable<string> toolNames, bool enabled)
        {
            if (toolNames == null)
            {
                throw new ArgumentNullException(nameof(toolNames));
            }

            string[] names = toolNames.ToArray();
            foreach (string toolName in names)
            {
                RequireKnown(toolName);
            }

            lock (gate)
            {
                foreach (string toolName in names)
                {
                    if (enabled)
                    {
                        disabledTools.Remove(toolName);
                    }
                    else
                    {
                        disabledTools.Add(toolName);
                    }
                }
            }
        }

        public IReadOnlyList<string> GetDisabledToolNames()
        {
            lock (gate)
            {
                return disabledTools.OrderBy(name => name, StringComparer.Ordinal).ToArray();
            }
        }

        public IReadOnlyList<string> GetEnabledToolNames()
        {
            lock (gate)
            {
                return knownTools
                    .Where(name => !disabledTools.Contains(name))
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        public void ValidateRegisteredTools(IEnumerable<string> registeredToolNames)
        {
            if (registeredToolNames == null)
            {
                throw new ArgumentNullException(nameof(registeredToolNames));
            }

            var registered = new HashSet<string>(registeredToolNames, StringComparer.Ordinal);
            if (!knownTools.SetEquals(registered))
            {
                string missing = string.Join(", ", knownTools.Except(registered));
                string unexpected = string.Join(", ", registered.Except(knownTools));
                throw new InvalidOperationException(
                    "The MCP presentation catalog and registered tools differ. "
                        + $"Missing: [{missing}]. Unexpected: [{unexpected}]."
                );
            }
        }

        private void RequireKnown(string toolName)
        {
            if (string.IsNullOrEmpty(toolName) || !knownTools.Contains(toolName))
            {
                throw new ArgumentException($"Unknown MCP tool '{toolName}'.", nameof(toolName));
            }
        }
    }
}
