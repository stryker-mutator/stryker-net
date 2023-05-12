using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.ProjectComponents.SourceProjects
{
    public class SourceProjectInfo : IProjectAndTests
    {
        private readonly List<string> _warnings = new();

        public IAnalyzerResult AnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public IProjectComponent ProjectContents { get; set; }
        public bool IsFullFramework => AnalyzerResult.TargetsFullFramework();
        public string HelperNamespace => CodeInjector.HelperNamespace;

        public CodeInjection CodeInjector { get;} = new();

        public TestProjectsInfo TestProjectsInfo { get; set; }

        public IReadOnlyCollection<string> Warnings => _warnings;

        public IReadOnlyList<string> GetTestAssemblies() =>
            TestProjectsInfo.GetTestAssemblies();

        public string LogError(string error)
        {
            _warnings.Add(error);
            return error;
        }
    }
}
