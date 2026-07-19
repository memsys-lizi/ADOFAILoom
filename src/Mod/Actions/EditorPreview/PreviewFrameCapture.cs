using System.Text.Json.Serialization;

namespace ADOFAILoom.Actions.EditorPreview
{
    internal sealed class PreviewFrameCapture
    {
        public PreviewFrameCapture(
            byte[] pngData,
            int width,
            int height,
            int floor,
            string revision
        )
        {
            PngData = pngData;
            Width = width;
            Height = height;
            Floor = floor;
            Revision = revision;
        }

        [JsonIgnore]
        public byte[] PngData { get; }

        public int Width { get; }

        public int Height { get; }

        public int Floor { get; }

        public string Revision { get; }
    }
}
