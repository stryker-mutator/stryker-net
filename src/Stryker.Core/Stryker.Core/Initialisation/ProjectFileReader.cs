using Buildalyzer;
using Microsoft.Build.Exceptions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        IAnalyzerResult AnalyzeProject(string projectFilepath, string solutionFilePath);
        IEnumerable<string> FindSharedProjects(XDocument document);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private readonly INugetRestoreProcess _nugetRestoreProcess;
        private readonly ILogger _logger;

        public ProjectFileReader(INugetRestoreProcess nugetRestoreProcess = null)
        {
            _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public IAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath)
        {
            AnalyzerManager manager;
            if (solutionFilePath == null)
            {
                manager = new AnalyzerManager();
            }
            else
            {
                _logger.LogDebug("Analyzing solution file {0}", solutionFilePath);
                try
                {
                    manager = new AnalyzerManager(solutionFilePath);
                }
                catch (InvalidProjectFileException)
                {
                    throw new StrykerInputException($"Incorrect solution path \"{solutionFilePath}\". Solution file not found. Please review your solution path setting.");
                }
            }

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = manager.GetProject(projectFilePath).Build().First();
            if (!analyzerResult.Succeeded)
            {
                if (!analyzerResult.TargetFramework.Contains("netcoreapp"))
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    _nugetRestoreProcess.RestorePackages(solutionFilePath);
                    analyzerResult = manager.GetProject(projectFilePath).Build().First();
                }
                else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogDebug("Project analyzer result not successful");
                }
            }

            return analyzerResult;
        }

        public IEnumerable<string> FindSharedProjects(XDocument document)
        {
            var importStatements = document.Elements().Descendants()
                .Where(projectElement => string.Equals(projectElement.Name.LocalName, "Import", StringComparison.OrdinalIgnoreCase));

            var sharedProjects = importStatements
                .SelectMany(importStatement => importStatement.Attributes(
                    XName.Get("Project")))
                .Select(importFileLocation => FilePathUtils.NormalizePathSeparators(importFileLocation.Value))
                .Where(importFileLocation => importFileLocation.EndsWith(".projitems"));
            return sharedProjects;
        }
    }
}
