using Stryker.Shared.Exceptions;

namespace Stryker.TestRunner.MSTest.Setup;

internal class DirectoryScanner
{
    public static string FindSolutionRoot(string path)
    {
        var directoryPath = Path.GetDirectoryName(path) ?? throw new GeneralStrykerException($"Could not load assembly from path: {path}");
        var directoryInfo = new DirectoryInfo(directoryPath);

        while (directoryInfo is not null && directoryInfo.GetFiles("*.sln").Length == 0)
        {
            directoryInfo = directoryInfo.Parent;
        }

        return directoryInfo?.FullName ?? string.Empty;
    }

    public static IEnumerable<string> FindCsprojFiles(string rootDirectory)
    {
        var directoryInfo = new DirectoryInfo(rootDirectory);

        var files = directoryInfo
            .GetFiles("*.csproj", SearchOption.AllDirectories)
            .Select(x => x.FullName);

        return files;
    }
}
