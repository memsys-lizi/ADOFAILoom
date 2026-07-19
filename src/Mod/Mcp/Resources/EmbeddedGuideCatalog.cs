using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ADOFAILoom.Mcp.Protocol;

namespace ADOFAILoom.Mcp.Resources
{
    internal sealed class EmbeddedGuideCatalog
    {
        private const string MimeType = "text/markdown; charset=utf-8";
        private readonly IReadOnlyList<McpResourceDefinition> definitions;
        private readonly IReadOnlyDictionary<string, McpTextResourceContents> contentsByUri;

        public EmbeddedGuideCatalog(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            GuideDescriptor[] descriptors =
            {
                new GuideDescriptor(
                    "index",
                    "README.md",
                    "ADOFAI 视效制作知识库",
                    "视效工作边界、阅读顺序、制作流程与强制原则。",
                    1.0
                ),
                new GuideDescriptor(
                    "foundations",
                    "FOUNDATIONS.md",
                    "视效设计基础",
                    "视觉层级、可读性、段落对比、状态管理与参数尺度。",
                    1.0
                ),
                new GuideDescriptor(
                    "camera",
                    "CAMERA.md",
                    "运镜指南",
                    "MoveCamera 的构图、坐标系、运动方式和复位规范。",
                    0.9
                ),
                new GuideDescriptor(
                    "decorations",
                    "DECORATIONS.md",
                    "装饰指南",
                    "装饰分层、标签、进入退出动画与资源使用规范。",
                    0.9
                ),
                new GuideDescriptor(
                    "tracks",
                    "TRACKS.md",
                    "轨道视效指南",
                    "轨道颜色、布局、动画、可读性与生命周期规范。",
                    0.9
                ),
                new GuideDescriptor(
                    "lighting-and-post",
                    "LIGHTING_AND_POST.md",
                    "光效与后处理指南",
                    "Flash、Bloom、滤镜、震动和屏幕效果的使用边界。",
                    0.9
                ),
                new GuideDescriptor(
                    "recipes",
                    "RECIPES.md",
                    "视效组合配方",
                    "常用段落、转场和强调效果的组合方法。",
                    0.8
                ),
                new GuideDescriptor(
                    "ai-checklist",
                    "AI_CHECKLIST.md",
                    "AI 执行与审查清单",
                    "调用编辑器工具前后的必检项、分批流程与保存规则。",
                    1.0
                ),
                new GuideDescriptor(
                    "workshop-study",
                    "WORKSHOP_STUDY.md",
                    "Workshop 样本研究",
                    "Workshop 关卡中的视效事件分布与可复用设计结论。",
                    0.7
                ),
            };

            var resourceDefinitions = new List<McpResourceDefinition>(descriptors.Length);
            var resourceContents = new Dictionary<string, McpTextResourceContents>(
                descriptors.Length,
                StringComparer.Ordinal
            );

            foreach (GuideDescriptor descriptor in descriptors)
            {
                string uri = $"adofailoom://guides/{descriptor.Slug}";
                string text = ReadEmbeddedText(assembly, descriptor.FileName);
                int size = Encoding.UTF8.GetByteCount(text);
                resourceDefinitions.Add(
                    new McpResourceDefinition(
                        uri,
                        descriptor.Slug,
                        descriptor.Title,
                        descriptor.Description,
                        MimeType,
                        size,
                        new McpResourceAnnotations(new[] { "assistant" }, descriptor.Priority)
                    )
                );
                resourceContents.Add(uri, new McpTextResourceContents(uri, MimeType, text));
            }

            definitions = resourceDefinitions;
            contentsByUri = resourceContents;
        }

        public IReadOnlyList<McpResourceDefinition> Definitions => definitions;

        public bool TryRead(string uri, out McpTextResourceContents? contents)
        {
            return contentsByUri.TryGetValue(uri, out contents);
        }

        private static string ReadEmbeddedText(Assembly assembly, string fileName)
        {
            string resourceName = $"ADOFAILoom.Guides.{fileName}";
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException(
                    $"Required embedded MCP guide '{resourceName}' was not found."
                );
            }

            using var reader = new StreamReader(
                stream,
                new UTF8Encoding(false, true),
                true,
                1024,
                false
            );
            return reader.ReadToEnd();
        }

        private sealed class GuideDescriptor
        {
            public GuideDescriptor(
                string slug,
                string fileName,
                string title,
                string description,
                double priority
            )
            {
                Slug = slug;
                FileName = fileName;
                Title = title;
                Description = description;
                Priority = priority;
            }

            public string Slug { get; }

            public string FileName { get; }

            public string Title { get; }

            public string Description { get; }

            public double Priority { get; }
        }
    }
}
