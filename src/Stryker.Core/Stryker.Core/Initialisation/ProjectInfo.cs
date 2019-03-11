using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        /// <summary>
        /// The path to the test project, the package was invoked in.
        /// </summary>
        public string TestProjectPath { get; set; }
        /// <summary>
        /// The filename of the test project file, the package was invoked in.
        /// </summary>
        public string TestProjectFileName { get; set; }
        /// <summary>
        /// The path to the project that should be mutated.
        /// </summary>
        public string ProjectUnderTestPath { get; set; }
        /// <summary>
        /// The AssemblyName of the project that should be mutated. Most of the time it is the same as ProjectUnderTestProjectName.
        /// </summary>
        public string ProjectUnderTestAssemblyName { get; set; }
        /// <summary>
        /// The name of the project that should be mutated
        /// </summary>
        public string ProjectUnderTestProjectName { get; set; }
        /// <summary>
        /// The target framework for the ProjectUnderTest.
        /// Default: netcoreapp2.0
        /// </summary>
        public string TargetFramework { get; set; }
        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }

        /// <summary>
        /// Indicates if Target Framework is append to output path.
        /// </summary>
        public bool AppendTargetFrameworkToOutputPath { get; set; }
    }
}
