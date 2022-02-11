using System.Collections.Generic;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public class AnalyzerResult : IAnalyzerResult
    {
        public AnalyzerResult(string projectFilePath, IEnumerable<string> references, IEnumerable<string> projectReferences, IEnumerable<string> analyzerReferences, IEnumerable<string> preprocessorSymbols, IReadOnlyDictionary<string, string> properties, string[] sourceFiles, bool succeeded, string targetFramework)
        {
            _projectFilePath = projectFilePath;
            _references = references;
            _projectReferences = projectReferences;
            _analyzerReferences = analyzerReferences;
            _preprocessorSymbols = preprocessorSymbols;
            _properties = properties;
            _sourceFiles = sourceFiles;
            _succeeded = succeeded;
            _targetFramework = targetFramework;
        }

        private readonly string _projectFilePath;
        private readonly IEnumerable<string> _references;
        private readonly IEnumerable<string> _projectReferences;
        private readonly IEnumerable<string> _analyzerReferences;
        private readonly IEnumerable<string> _preprocessorSymbols;
        private readonly IReadOnlyDictionary<string, string> _properties;
        private readonly string[] _sourceFiles;
        private readonly bool _succeeded;
        private readonly string _targetFramework;


        public string ProjectFilePath { get { return _projectFilePath; } }

        public IEnumerable<string> References { get { return _references; } }

        public IEnumerable<string> ProjectReferences { get { return _projectReferences; } }

        public IEnumerable<string> AnalyzerReferences { get { return _analyzerReferences; } }

        public IEnumerable<string> PreprocessorSymbols { get { return _preprocessorSymbols; } }

        public IReadOnlyDictionary<string, string> Properties { get { return _properties; } }

        public string[] SourceFiles { get { return _sourceFiles; } }

        public bool Succeeded { get { return _succeeded; } }

        public string TargetFramework { get { return _targetFramework; } }
    }
}
