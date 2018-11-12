using System.IO;

public static class FilePathUtils
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

    public static string ConvertToPlatformSupportedFilePath(string filePath)
    {
        return filePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }
}