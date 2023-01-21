using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Stryker.UnitySDK
{
    [InitializeOnLoad]
    public static class ModifyCsproj
    {
        static ModifyCsproj()
        {
            foreach (var fileInfo in Directory.GetFiles(Path.Combine(Application.dataPath, ".."), "*.csproj")!)
            {
                UpdateOutputPathForUnityProjects(fileInfo);
            }
        }

        private static void UpdateOutputPathForUnityProjects(string projectPath)
        {
            var pattern = new Regex(@"<OutputPath>.*<\/OutputPath>", RegexOptions.Compiled);
            var updatedCsProjContent = pattern.Replace(File.ReadAllText(projectPath),
                _ => @"<OutputPath>Library\ScriptAssemblies\</OutputPath>");
            File.WriteAllText(projectPath, updatedCsProjContent);
            Debug.Log(projectPath + " was updated ");
        }
    }
}
