using System.IO;

namespace Stryker.Core
{
    public static class FilePathUtils
    {
        private static readonly char[] _knownDirectorySeparators = { '\\', '/' };

        public static string NormalizePathSeparators(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }

            return string.Join(Path.DirectorySeparatorChar, filePath.Split(_knownDirectorySeparators));
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