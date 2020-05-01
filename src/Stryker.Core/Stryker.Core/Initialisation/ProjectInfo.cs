using Buildalyzer;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.IO;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public IEnumerable<IAnalyzerResult> TestProjectAnalyzerResults { get; set; }
        public IAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }

        public string GetInjectionPath(IAnalyzerResult analyzerResult)
        {
            return Path.Combine(
                Path.GetDirectoryName(GetAssemblyPath(analyzerResult)), 
                Path.GetFileName(GetAssemblyPath(ProjectUnderTestAnalyzerResult)));
        }

        public string GetAssemblyPath(IAnalyzerResult analyzerResult)
        {
            return FilePathUtils.NormalizePathSeparators(Path.Combine(
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]),
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"])));
        }
    }

    public enum Framework
    {
        NetClassic,
        NetCore,
        NetStandard,
        Unknown
    };
}
