using System.Collections.Generic;
using System.IO;
using Buildalyzer;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public IEnumerable<IAnalyzerResult> TestProjectAnalyzerResults { get; set; }
        public IAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public IProjectComponent ProjectContents { get; set; }

        public string GetInjectionFilePath(IAnalyzerResult analyzerResult)
        {
            return Path.Combine(
                Path.GetDirectoryName(analyzerResult.GetAssemblyPath()),
                Path.GetFileName(ProjectUnderTestAnalyzerResult.GetAssemblyPath()));
        }

        public void RestoreOrginalAssembly()
        {
            foreach(var testProject in TestProjectAnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject);
                File.Delete(injectionPath);
                File.Move(injectionPath + ".stryker-unchanged", injectionPath);
            }
        }
    }

    public enum Framework
    {
        DotNetClassic,
        DotNet,
        DotNetStandard,
        Unknown
    };
}
