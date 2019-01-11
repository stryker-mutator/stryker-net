using Buildalyzer;
using Stryker.Core.Initialisation.ProjectComponent;
using System.Collections.Generic;
using System.IO;

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

        public string GetInjectionPath()
        {
            var outputPath = ProjectUnderTestAnalyzerResult.Properties.GetValueOrDefault("OutputPath");
            var assemblyName = ProjectUnderTestAnalyzerResult.Properties.GetValueOrDefault("AssemblyName");
            string injectionPath = Path.Combine(outputPath, assemblyName + ".dll");
            return injectionPath;
        }
    }
}
