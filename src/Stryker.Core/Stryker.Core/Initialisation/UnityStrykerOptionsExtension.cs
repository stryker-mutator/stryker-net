using System.IO;
using System.Linq;
using Stryker.Core.Options;

namespace Stryker.Core.Initialisation;

public static class UnityStrykerOptionsExtension
{
    public static bool IsUnityProject(this StrykerOptions options)
    {
        if (options == null)
            return false;

        var path = options.GetUnityProjectDirectory();

        var unityCsProjFile = Directory
            .GetFiles(path, "Assembly-CSharp.csproj",
                SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (string.IsNullOrEmpty(unityCsProjFile))
        {
            return false;
        }

        var containsUnityEngineReferences =
            File.ReadAllText(unityCsProjFile).Contains("<Reference Include=\"UnityEngine.");
        return containsUnityEngineReferences;
    }

    public static string GetUnityProjectDirectory(this StrykerOptions options)
    {
        if (options == null)
            return null;

        var path = options.ProjectPath ?? options.SolutionPath;
        var isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        if (isDirectory)
        {
            return path;
        }
        else
        {
            return Directory.GetParent(path)!.FullName;
        }
    }
}
