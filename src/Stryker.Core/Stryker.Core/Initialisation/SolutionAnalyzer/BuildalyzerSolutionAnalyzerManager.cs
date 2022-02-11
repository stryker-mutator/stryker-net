using System.Collections.Generic;
using Buildalyzer;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public class BuildalyzerSolutionAnalyzerManager : ISolutionAnalyzerManager
    {
        private readonly Dictionary<string, IProjectAnalyzer> _projects = new Dictionary<string, IProjectAnalyzer>();

        public BuildalyzerSolutionAnalyzerManager(string solutionFilePath)
        {
            AnalyzerManager analyzerManager = new(solutionFilePath);

            foreach((string name, Buildalyzer.IProjectAnalyzer projectAnalyzer) in analyzerManager.Projects)
            {
                _projects.Add(name, new BuildalyzerProjectAnalyzer(projectAnalyzer));
            }
        }

        public IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get { return _projects; } }
    }
}
