using Buildalyzer;

namespace Stryker.Core.ProjectComponents.TargetProjects
{
    public class TargetProjectInfo
    {
        public IAnalyzerResult AnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public IProjectComponent ProjectContents { get; set; }
    }
}
