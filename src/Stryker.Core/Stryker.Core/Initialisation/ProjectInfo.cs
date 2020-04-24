using Buildalyzer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
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
