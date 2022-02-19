using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class AnalyzerResult : IAnalysisResult
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
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AnalyzerResult>();
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
        private readonly ILogger _logger;


        public string ProjectFilePath { get { return _projectFilePath; } }

        public IEnumerable<string> References { get { return _references; } }

        public IEnumerable<string> ProjectReferences { get { return _projectReferences; } }

        public IEnumerable<string> AnalyzerReferences { get { return _analyzerReferences; } }

        public IEnumerable<string> PreprocessorSymbols { get { return _preprocessorSymbols; } }

        public IReadOnlyDictionary<string, string> Properties { get { return _properties; } }

        public string[] SourceFiles { get { return _sourceFiles; } }

        public bool Succeeded { get { return _succeeded; } }

        public string TargetFramework { get { return _targetFramework; } }

        public void Log()
        {
            // dump all properties as it can help diagnosing build issues for user project.
            _logger.LogTrace("**** Analyzer result ****");

            _logger.LogTrace("Project: {0}", ProjectFilePath);
            _logger.LogTrace("TargetFramework: {0}", TargetFramework);

            foreach (var property in Properties ?? new Dictionary<string, string>())
            {
                _logger.LogTrace("Property {0}={1}", property.Key, property.Value);
            }
            foreach (var sourceFile in SourceFiles ?? Enumerable.Empty<string>())
            {
                _logger.LogTrace("SourceFile {0}", sourceFile);
            }
            foreach (var reference in References ?? Enumerable.Empty<string>())
            {
                _logger.LogTrace("References: {0}", reference);
            }
            _logger.LogTrace("Succeeded: {0}", Succeeded);
            _logger.LogTrace("**** Analyzer result ****");
        }
    }
}
