using System.IO;

namespace Stryker.Core
{
    public static class FilePathUtils
    {
        public static string ConvertPathSeparators(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }

            const char windowsDirectorySeparator = '\\';
            if (Path.DirectorySeparatorChar == windowsDirectorySeparator)
            {
                return filePath;
            }
            else
            {
                return filePath.Replace(windowsDirectorySeparator, Path.DirectorySeparatorChar);
            }
        }

        public static string ToFullPath(this string path)
        {
            if (path == null)
            {
                return null;
            }

            return Path.GetFullPath(path);
        }
    }
}