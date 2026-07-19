using System;

namespace ADOFAILoom.Mcp.Tooling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class McpToolAttribute : Attribute
    {
        public McpToolAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public string Description { get; set; } = string.Empty;

        public bool ReadOnly { get; set; }

        public bool Destructive { get; set; } = true;

        public bool Idempotent { get; set; }

        public bool OpenWorld { get; set; } = true;
    }

    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false
    )]
    internal sealed class McpOptionalAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class McpImageContentAttribute : Attribute
    {
        public McpImageContentAttribute(string dataProperty, string mimeType)
        {
            DataProperty = dataProperty ?? throw new ArgumentNullException(nameof(dataProperty));
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
        }

        public string DataProperty { get; }

        public string MimeType { get; }
    }
}
