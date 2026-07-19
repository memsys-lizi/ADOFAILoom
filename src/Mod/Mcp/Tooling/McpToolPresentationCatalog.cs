using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ADOFAILoom.Mcp.Tooling
{
    internal static class McpToolPresentationCatalog
    {
        private static readonly IReadOnlyDictionary<string, DirectPresentation> DirectTools =
            new Dictionary<string, DirectPresentation>(StringComparer.Ordinal)
            {
                ["get_game_state"] = new DirectPresentation(
                    ToolCategory.Game,
                    "读取游戏状态",
                    "读取当前场景、游戏模式和暂停状态。"
                ),
                ["switch_scene"] = new DirectPresentation(
                    ToolCategory.Navigation,
                    "切换场景",
                    "切换到准确的 Unity 场景，例如编辑器、自定义选关或官方选关。"
                ),
                ["open_level"] = new DirectPresentation(
                    ToolCategory.Navigation,
                    "打开关卡",
                    "在当前编辑器中打开指定的绝对 .adofai 文件路径。"
                ),
                ["get_editor_state"] = new DirectPresentation(
                    ToolCategory.EditorState,
                    "读取编辑器状态",
                    "读取当前关卡路径、楼层、选择、预览状态、撤销状态和 revision。"
                ),
                ["list_visual_events"] = new DirectPresentation(
                    ToolCategory.EditorState,
                    "列出视效事件",
                    "分页读取视觉事件、装饰事件、属性和用于更新或删除的严格引用。"
                ),
                ["undo_editor"] = new DirectPresentation(
                    ToolCategory.Workflow,
                    "撤销编辑",
                    "使用游戏原生历史栈撤销一个编辑器修改批次。"
                ),
                ["redo_editor"] = new DirectPresentation(
                    ToolCategory.Workflow,
                    "重做编辑",
                    "使用游戏原生历史栈重做一个编辑器修改批次。"
                ),
                ["save_level"] = new DirectPresentation(
                    ToolCategory.Workflow,
                    "保存关卡",
                    "保存当前关卡，并重新读取磁盘文件验证写入后的 revision。"
                ),
                ["start_visual_preview"] = new DirectPresentation(
                    ToolCategory.Preview,
                    "开始视效预览",
                    "从指定的零基楼层启动编辑器原生游玩预览。"
                ),
                ["stop_visual_preview"] = new DirectPresentation(
                    ToolCategory.Preview,
                    "停止视效预览",
                    "停止当前编辑器预览并返回编辑模式。"
                ),
                ["capture_preview_frame"] = new DirectPresentation(
                    ToolCategory.Preview,
                    "截取预览画面",
                    "把当前预览画面作为标准 MCP PNG 图片返回给模型。"
                ),
            };

        private static readonly IReadOnlyDictionary<string, EventPresentation> EventFamilies =
            new Dictionary<string, EventPresentation>(StringComparer.Ordinal)
            {
                ["camera_move_events"] = new EventPresentation(ToolCategory.Camera, "镜头移动事件"),
                ["filter_events"] = new EventPresentation(ToolCategory.Filters, "普通滤镜事件"),
                ["flash_events"] = new EventPresentation(ToolCategory.Screen, "闪光事件"),
                ["bloom_events"] = new EventPresentation(ToolCategory.Screen, "Bloom 事件"),
                ["shake_screen_events"] = new EventPresentation(
                    ToolCategory.Screen,
                    "屏幕震动事件"
                ),
                ["screen_tile_events"] = new EventPresentation(ToolCategory.Screen, "屏幕平铺事件"),
                ["screen_scroll_events"] = new EventPresentation(
                    ToolCategory.Screen,
                    "屏幕滚动事件"
                ),
                ["hall_of_mirrors_events"] = new EventPresentation(ToolCategory.Screen, "镜厅事件"),
                ["set_frame_rate_events"] = new EventPresentation(ToolCategory.Screen, "帧率事件"),
                ["custom_background_events"] = new EventPresentation(
                    ToolCategory.Background,
                    "自定义背景事件"
                ),
                ["color_track_events"] = new EventPresentation(ToolCategory.Track, "轨道颜色事件"),
                ["animate_track_events"] = new EventPresentation(
                    ToolCategory.Track,
                    "轨道动画事件"
                ),
                ["recolor_track_events"] = new EventPresentation(
                    ToolCategory.Track,
                    "轨道重着色事件"
                ),
                ["move_track_events"] = new EventPresentation(ToolCategory.Track, "轨道移动事件"),
                ["position_track_events"] = new EventPresentation(
                    ToolCategory.Track,
                    "轨道定位事件"
                ),
                ["tile_dimensions_events"] = new EventPresentation(
                    ToolCategory.Track,
                    "砖块尺寸事件"
                ),
                ["set_floor_icon_events"] = new EventPresentation(
                    ToolCategory.Track,
                    "楼层图标事件"
                ),
                ["text_decorations"] = new EventPresentation(ToolCategory.Text, "文本装饰"),
                ["set_text_events"] = new EventPresentation(ToolCategory.Text, "文本控制事件"),
                ["default_text_events"] = new EventPresentation(ToolCategory.Text, "默认文本事件"),
                ["image_decorations"] = new EventPresentation(ToolCategory.Decorations, "图片装饰"),
                ["move_decoration_events"] = new EventPresentation(
                    ToolCategory.Decorations,
                    "装饰移动事件"
                ),
                ["object_decorations"] = new EventPresentation(ToolCategory.Objects, "对象装饰"),
                ["set_object_events"] = new EventPresentation(ToolCategory.Objects, "对象控制事件"),
                ["particle_decorations"] = new EventPresentation(
                    ToolCategory.Particles,
                    "粒子装饰"
                ),
                ["emit_particle_events"] = new EventPresentation(
                    ToolCategory.Particles,
                    "粒子发射事件"
                ),
                ["set_particle_events"] = new EventPresentation(
                    ToolCategory.Particles,
                    "粒子控制事件"
                ),
            };

        public static IReadOnlyList<McpToolPresentation> Discover(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            McpToolPresentation[] tools = assembly
                .GetTypes()
                .SelectMany(type =>
                    type.GetMethods(
                        BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.Static
                            | BindingFlags.DeclaredOnly
                    )
                )
                .Select(method => method.GetCustomAttribute<McpToolAttribute>())
                .Where(attribute => attribute != null)
                .Select(attribute => Create(attribute!))
                .OrderBy(tool => tool.Category.Order)
                .ThenBy(tool => tool.Name, StringComparer.Ordinal)
                .ToArray();

            if (tools.Length == 0)
            {
                throw new InvalidOperationException("No MCP tools were found for the UMM panel.");
            }

            string? duplicate = tools
                .GroupBy(tool => tool.Name, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .FirstOrDefault();
            if (duplicate != null)
            {
                throw new InvalidOperationException(
                    $"Duplicate MCP tool presentation name '{duplicate}'."
                );
            }

            return tools;
        }

        private static McpToolPresentation Create(McpToolAttribute attribute)
        {
            if (DirectTools.TryGetValue(attribute.Name, out DirectPresentation? direct))
            {
                return new McpToolPresentation(
                    attribute.Name,
                    direct.Category,
                    direct.Title,
                    direct.Description
                );
            }

            string action;
            string familyName;
            if (attribute.Name.StartsWith("add_", StringComparison.Ordinal))
            {
                action = "新增";
                familyName = attribute.Name.Substring("add_".Length);
            }
            else if (attribute.Name.StartsWith("update_", StringComparison.Ordinal))
            {
                action = "更新";
                familyName = attribute.Name.Substring("update_".Length);
            }
            else if (attribute.Name.StartsWith("remove_", StringComparison.Ordinal))
            {
                action = "删除";
                familyName = attribute.Name.Substring("remove_".Length);
            }
            else
            {
                throw UnknownPresentation(attribute.Name);
            }

            if (!EventFamilies.TryGetValue(familyName, out EventPresentation? family))
            {
                throw UnknownPresentation(attribute.Name);
            }

            string description = action switch
            {
                "新增" => $"按请求顺序批量新增{family.Title}；成功批次只产生一个撤销步骤。",
                "更新" => $"使用当前 revision 和严格事件引用批量更新{family.Title}。",
                "删除" => $"使用当前 revision 和严格事件引用批量删除{family.Title}。",
                _ => throw new InvalidOperationException($"Unsupported tool action '{action}'."),
            };
            return new McpToolPresentation(
                attribute.Name,
                family.Category,
                action + family.Title,
                description
            );
        }

        private static InvalidOperationException UnknownPresentation(string toolName)
        {
            return new InvalidOperationException(
                $"MCP tool '{toolName}' has no strict UMM category and Chinese description."
            );
        }

        private sealed class DirectPresentation
        {
            public DirectPresentation(ToolCategory category, string title, string description)
            {
                Category = category;
                Title = title;
                Description = description;
            }

            public ToolCategory Category { get; }

            public string Title { get; }

            public string Description { get; }
        }

        private sealed class EventPresentation
        {
            public EventPresentation(ToolCategory category, string title)
            {
                Category = category;
                Title = title;
            }

            public ToolCategory Category { get; }

            public string Title { get; }
        }
    }

    internal sealed class McpToolPresentation
    {
        public McpToolPresentation(
            string name,
            ToolCategory category,
            string title,
            string description
        )
        {
            Name = name;
            Category = category;
            Title = title;
            Description = description;
        }

        public string Name { get; }

        public ToolCategory Category { get; }

        public string Title { get; }

        public string Description { get; }
    }

    internal sealed class ToolCategory
    {
        private ToolCategory(string id, string displayName, int order)
        {
            Id = id;
            DisplayName = displayName;
            Order = order;
        }

        public static ToolCategory Game { get; } = new ToolCategory("game", "游戏状态", 0);

        public static ToolCategory Navigation { get; } =
            new ToolCategory("navigation", "场景与关卡", 10);

        public static ToolCategory EditorState { get; } =
            new ToolCategory("editor_state", "编辑器读取", 20);

        public static ToolCategory Camera { get; } = new ToolCategory("camera", "镜头", 30);

        public static ToolCategory Filters { get; } = new ToolCategory("filters", "滤镜", 40);

        public static ToolCategory Screen { get; } = new ToolCategory("screen", "屏幕效果", 50);

        public static ToolCategory Background { get; } = new ToolCategory("background", "背景", 60);

        public static ToolCategory Track { get; } = new ToolCategory("track", "轨道", 70);

        public static ToolCategory Text { get; } = new ToolCategory("text", "文本", 80);

        public static ToolCategory Decorations { get; } =
            new ToolCategory("decorations", "图片装饰", 90);

        public static ToolCategory Objects { get; } = new ToolCategory("objects", "对象", 100);

        public static ToolCategory Particles { get; } = new ToolCategory("particles", "粒子", 110);

        public static ToolCategory Preview { get; } =
            new ToolCategory("preview", "预览与截图", 120);

        public static ToolCategory Workflow { get; } =
            new ToolCategory("workflow", "撤销、重做与保存", 130);

        public string Id { get; }

        public string DisplayName { get; }

        public int Order { get; }
    }
}
