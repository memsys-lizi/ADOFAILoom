using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAILoom.Mcp.Tooling;
using UnityEngine;

namespace ADOFAILoom.Settings
{
    internal sealed class ToolSettingsPanel
    {
        private const float PanelHeight = 620f;

        private readonly IReadOnlyList<McpToolPresentation> tools;
        private readonly McpToolAvailability availability;
        private readonly HashSet<string> expandedCategories = new HashSet<string>(
            StringComparer.Ordinal
        );

        private Vector2 scrollPosition;

        public ToolSettingsPanel(
            IReadOnlyList<McpToolPresentation> tools,
            McpToolAvailability availability
        )
        {
            this.tools = tools ?? throw new ArgumentNullException(nameof(tools));
            this.availability =
                availability ?? throw new ArgumentNullException(nameof(availability));

            expandedCategories.Add(ToolCategory.Game.Id);
            expandedCategories.Add(ToolCategory.Navigation.Id);
            expandedCategories.Add(ToolCategory.EditorState.Id);
        }

        public void Draw()
        {
            GUIStyle descriptionStyle = CreateDescriptionStyle();
            int enabledCount = tools.Count(tool => availability.IsEnabled(tool.Name));

            GUILayout.BeginVertical("box");
            GUILayout.Label("MCP 工具开关");
            GUILayout.Label(
                $"已启用 {enabledCount} / {tools.Count}。关闭后工具会立即从 tools/list 消失，"
                    + "并拒绝新的调用；请在客户端刷新 MCP 工具列表。",
                descriptionStyle
            );
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("全部启用", GUILayout.Width(120f)))
            {
                availability.SetEnabled(tools.Select(tool => tool.Name), true);
            }

            if (GUILayout.Button("全部禁用", GUILayout.Width(120f)))
            {
                availability.SetEnabled(tools.Select(tool => tool.Name), false);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(PanelHeight)
            );
            foreach (
                IGrouping<ToolCategory, McpToolPresentation> category in tools.GroupBy(tool =>
                    tool.Category
                )
            )
            {
                DrawCategory(category.Key, category.ToArray(), descriptionStyle);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawCategory(
            ToolCategory category,
            IReadOnlyList<McpToolPresentation> categoryTools,
            GUIStyle descriptionStyle
        )
        {
            int enabledCount = categoryTools.Count(tool => availability.IsEnabled(tool.Name));
            bool expanded = expandedCategories.Contains(category.Id);
            string indicator = expanded ? "▼" : "▶";
            if (
                GUILayout.Button(
                    $"{indicator} {category.DisplayName}  ({enabledCount}/{categoryTools.Count})"
                )
            )
            {
                if (expanded)
                {
                    expandedCategories.Remove(category.Id);
                }
                else
                {
                    expandedCategories.Add(category.Id);
                }

                expanded = !expanded;
            }

            if (!expanded)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(18f);
            if (GUILayout.Button("本类全开", GUILayout.Width(100f)))
            {
                availability.SetEnabled(categoryTools.Select(tool => tool.Name), true);
            }

            if (GUILayout.Button("本类全关", GUILayout.Width(100f)))
            {
                availability.SetEnabled(categoryTools.Select(tool => tool.Name), false);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach (McpToolPresentation tool in categoryTools)
            {
                DrawTool(tool, descriptionStyle);
            }

            GUILayout.Space(6f);
        }

        private void DrawTool(McpToolPresentation tool, GUIStyle descriptionStyle)
        {
            GUILayout.BeginVertical("box");
            bool enabled = availability.IsEnabled(tool.Name);
            bool nextEnabled = GUILayout.Toggle(enabled, $"{tool.Title}  [{tool.Name}]");
            if (nextEnabled != enabled)
            {
                availability.SetEnabled(tool.Name, nextEnabled);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(24f);
            GUILayout.Label(tool.Description, descriptionStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static GUIStyle CreateDescriptionStyle()
        {
            return new GUIStyle(GUI.skin.label) { wordWrap = true };
        }
    }
}
