using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Stryker.UnitySDK
{
    public class CSProjPostprocessor : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            var pattern = new Regex(@"<OutputPath>.*<\/OutputPath>", RegexOptions.Compiled);
            var updatedCsProjContent = pattern.Replace(content,
                _ => @"<OutputPath>Library\ScriptAssemblies\</OutputPath>");
            Debug.Log(path + " was updated OutputPath");
            return updatedCsProjContent;
        }
    }
}
