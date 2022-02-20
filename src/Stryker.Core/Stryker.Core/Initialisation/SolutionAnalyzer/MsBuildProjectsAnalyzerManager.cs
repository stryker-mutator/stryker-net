using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Stryker.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Buildalyzer;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class MsBuildProjectsAnalyzerManager : IProjectsAnalyzerManager
    {
        private readonly Dictionary<string, IProjectAnalyzer> _projects = new Dictionary<string, IProjectAnalyzer>();
        private readonly ILogger _logger;

        internal static readonly SolutionProjectType[] SupportedProjectTypes = new SolutionProjectType[]
        {
            SolutionProjectType.KnownToBeMSBuildFormat,
            SolutionProjectType.WebProject
        };

        public MsBuildProjectsAnalyzerManager(string filePath)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MsBuildProjectsAnalyzerManager>();
            _logger.LogDebug("Analyzing file {0}", filePath);

            if (!File.Exists(filePath))
            {
                throw new InputException($"Incorrect file path \"{filePath}\". File not found.");
            }

            if (Path.GetExtension(filePath).EndsWith(@".sln", StringComparison.Ordinal))
            {
                var solutionFile = SolutionFile.Parse(filePath);

                // Initialize all the projects in the solution
                foreach (ProjectInSolution projectInSolution in solutionFile.ProjectsInOrder)
                {
                    if (!SupportedProjectTypes.Contains(projectInSolution.ProjectType))
                    {
                        continue;
                    }
                    if (!File.Exists(projectInSolution.AbsolutePath))
                    {
                        continue;
                    }
                    _projects.Add(filePath, new MsBuildProjectAnalyzer(projectInSolution));
                }
            }
            else
            {
                _projects.Add(filePath, new MsBuildProjectAnalyzer(filePath));
            }
        }
        public IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get { return _projects; } }
    }
}
