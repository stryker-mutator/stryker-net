using Buildalyzer;
using Stryker.Core.Initialisation.ProjectComponent;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public AnalyzerResult TestProjectAnalyzerResult { get; set; }
        public AnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }
        public bool FullFramework { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }
        
    }
}
