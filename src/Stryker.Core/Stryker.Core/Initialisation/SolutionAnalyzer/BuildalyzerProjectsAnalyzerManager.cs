using System;
using System.Collections.Generic;
using System.IO;
using Buildalyzer;
using Microsoft.Build.Exceptions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class BuildalyzerProjectsAnalyzerManager : IProjectsAnalyzerManager
    {
        private readonly Dictionary<string, IProjectAnalyzer> _projects = new Dictionary<string, IProjectAnalyzer>();
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">.sln or Project file</param>
        public BuildalyzerProjectsAnalyzerManager(string filePath)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<BuildalyzerProjectsAnalyzerManager>();

            try
            {
                _logger.LogDebug("Analyzing file {0}", filePath);
                if (Path.GetExtension(filePath).EndsWith(@".sln", StringComparison.Ordinal))
                {
                    AnalyzerManager analyzerManager = new(filePath);
                    foreach ((string name, Buildalyzer.IProjectAnalyzer projectAnalyzer) in analyzerManager.Projects)
                    {
                        _projects.Add(name, new BuildalyzerProjectAnalyzer(projectAnalyzer));
                    }
                }
                else
                {
                    AnalyzerManager analyzerManager = new();
                    Buildalyzer.IProjectAnalyzer projectAnalyzer = analyzerManager.GetProject(filePath);
                    _projects.Add(filePath, new BuildalyzerProjectAnalyzer(projectAnalyzer));
                }
            }
            catch (InvalidProjectFileException)
            {
                throw new InputException($"Incorrect path \"{filePath}\". File not found. Please review your path setting.");
            }
        }

        public IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get { return _projects; } }
    }
}
