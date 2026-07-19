using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace ADOFAILoom.Settings
{
    public sealed class ToolSettings : UnityModManager.ModSettings
    {
        public List<string> DisabledTools = new List<string>();

        public static ToolSettings LoadStrict(UnityModManager.ModEntry modEntry)
        {
            var empty = new ToolSettings();
            string path = empty.GetPath(modEntry);
            if (!File.Exists(path))
            {
                return empty;
            }

            using FileStream stream = File.OpenRead(path);
            object? value = new XmlSerializer(typeof(ToolSettings)).Deserialize(stream);
            return value as ToolSettings
                ?? throw new InvalidDataException(
                    $"UMM settings file '{path}' does not contain ToolSettings."
                );
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            string path = GetPath(modEntry);
            string temporaryPath = path + ".tmp";
            try
            {
                using (var writer = new StreamWriter(temporaryPath, false))
                {
                    new XmlSerializer(typeof(ToolSettings)).Serialize(writer, this);
                }

                if (File.Exists(path))
                {
                    File.Replace(temporaryPath, path, null);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
            }
            catch
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }

                throw;
            }
        }
    }
}
