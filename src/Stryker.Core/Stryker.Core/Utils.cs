using System.IO;

public static class Utilities
{
    public static string ConvertPathSeparators(string filePath)
    {
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
}