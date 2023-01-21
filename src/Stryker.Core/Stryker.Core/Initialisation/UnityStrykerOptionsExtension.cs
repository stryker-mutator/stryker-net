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

        var unityCsProjFile = Directory
            .GetFiles(options.ProjectPath ?? options.SolutionPath, "Assembly-CSharp.csproj",
                SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (string.IsNullOrEmpty(unityCsProjFile))
        {
            return false;
        }

        var containsUnityEngineReferences =
            File.ReadAllText(unityCsProjFile).Contains("<Reference Include=\"UnityEngine.");
        return containsUnityEngineReferences;
    }
}
