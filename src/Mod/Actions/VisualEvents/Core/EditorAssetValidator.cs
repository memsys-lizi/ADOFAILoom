using System;
using System.IO;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal static class EditorAssetValidator
    {
        public static void ValidateExistingLevelAsset(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                return;
            }

            if (
                Path.IsPathRooted(fileName)
                || !string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal)
                || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            )
            {
                throw new ArgumentException(
                    "Level asset references must be exact file names in the level directory.",
                    nameof(fileName)
                );
            }

            string levelPath = ADOBase.levelPath;
            if (string.IsNullOrEmpty(levelPath) || !Path.IsPathRooted(levelPath))
            {
                throw new InvalidOperationException(
                    "The editor level must have an absolute .adofai path before referencing assets."
                );
            }

            string? levelDirectory = Path.GetDirectoryName(Path.GetFullPath(levelPath));
            if (levelDirectory == null)
            {
                throw new InvalidOperationException(
                    "The editor level path does not have a parent directory."
                );
            }

            string assetPath = Path.Combine(levelDirectory, fileName);
            if (!File.Exists(assetPath))
            {
                throw new FileNotFoundException(
                    $"The level asset '{fileName}' does not exist in '{levelDirectory}'.",
                    assetPath
                );
            }
        }
    }
}
