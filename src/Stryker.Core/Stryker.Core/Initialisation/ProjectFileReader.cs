using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        ProjectAnalyzerResult AnalyzeProject(string projectFilepath, string solutionFilePath);
        string DetermineProjectUnderTest(IEnumerable<string> projectReferences, string projectUnderTestNameFilter);
        IEnumerable<string> FindSharedProjects(XDocument document);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private const string ErrorMessage = "Project reference issue.";
        private IProcessExecutor _processExecutor { get; set; }
        private ILogger _logger { get; set; }

        public ProjectFileReader(IProcessExecutor processExecutor = null)
        {
            _processExecutor = processExecutor ?? new ProcessExecutor();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public ProjectAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath)
        {
            AnalyzerManager manager;
            if (solutionFilePath == null)
            {
                manager = new AnalyzerManager();
            }
            else
            {
                _logger.LogDebug("Analyzing solution file {0}", solutionFilePath);
                manager = new AnalyzerManager(solutionFilePath);
            }

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = manager.GetProject(projectFilePath).Build().First();
            if (!analyzerResult.Succeeded)
            {
                if (!analyzerResult.TargetFramework.Contains("netcoreapp"))
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    RestorePackages(solutionFilePath);
                    analyzerResult = manager.GetProject(projectFilePath).Build().First();
                } else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogWarning("Project analyzer result not successful");
                }
            }

            return new ProjectAnalyzerResult(_logger, analyzerResult);
        }

        public IEnumerable<string> FindSharedProjects(XDocument document)
        {
            var projectReferenceElements = document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "Import", StringComparison.OrdinalIgnoreCase));
            return projectReferenceElements.SelectMany(x => x.Attributes(XName.Get("Project"))).Select(y => FilePathUtils.ConvertPathSeparators(y.Value));
        }

        public string DetermineProjectUnderTest(IEnumerable<string> projectReferences, string projectUnderTestNameFilter)
        {
            var referenceChoise = BuildReferenceChoise(projectReferences);

            var stringBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(projectUnderTestNameFilter))
            {
                if (projectReferences.Count() > 1)
                {
                    stringBuilder.AppendLine("Test project contains more than one project references. Please add the --project-file=[projectname] argument to specify which project to mutate.");
                    stringBuilder.Append(referenceChoise);
                    AppendExampleIfPossible(stringBuilder, projectReferences);

                    throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
                }

                if (!projectReferences.Any())
                {
                    stringBuilder.AppendLine("No project references found. Please add a project reference to your test project and retry.");

                    throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
                }

                return projectReferences.Single();
            }
            else
            {
                var searchResult = projectReferences.Where(x => x.ToLower().Contains(projectUnderTestNameFilter.ToLower())).ToList();
                if (!searchResult.Any())
                {
                    stringBuilder.Append("No project reference matched your --project-file=");
                    stringBuilder.AppendLine(projectUnderTestNameFilter);
                    stringBuilder.Append(referenceChoise);
                    AppendExampleIfPossible(stringBuilder, projectReferences, projectUnderTestNameFilter);

                    throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
                }
                else if (searchResult.Count() > 1)
                {
                    stringBuilder.Append("More than one project reference matched your --project-file=");
                    stringBuilder.Append(projectUnderTestNameFilter);
                    stringBuilder.AppendLine(" argument to specify the project to mutate, please specify the name more detailed.");
                    stringBuilder.Append(referenceChoise);
                    AppendExampleIfPossible(stringBuilder, projectReferences, projectUnderTestNameFilter);

                    throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
                }
                return FilePathUtils.ConvertPathSeparators(searchResult.Single());
            }
        }

        private void RestorePackages(string solutionPath)
        {
            _logger.LogInformation("Restoring nuget packages using {0}", "nuget.exe");
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new StrykerInputException("Solution path is required on .net framework projects. Please provide your solution path using --solution-path ...");
            }
            solutionPath = Path.GetFullPath(solutionPath);
            string solutionDir = Path.GetDirectoryName(solutionPath);

            // Validate nuget.exe is installed and included in path
            var nugetWhereExeResult = _processExecutor.Start(solutionDir, "where.exe", "nuget.exe");
            if (!nugetWhereExeResult.Output.Contains("nuget.exe"))
            {
                throw new StrykerInputException("Nuget.exe should be installed to restore .net framework nuget packages. Install nuget.exe and make sure it's included in your path.");
            }

            // Locate MSBuild.exe
            var msbuildPath = new MsBuildHelper().GetMsBuildPath();
            var msBuildVersionOutput = _processExecutor.Start(solutionDir, msbuildPath, "-version /nologo");
            if (msBuildVersionOutput.ExitCode != 0)
            {
                _logger.LogError("Unable to detect msbuild version");
            }
            _logger.LogDebug("Auto detected msbuild version {0} at: {1}", msBuildVersionOutput.Output.Trim(), msbuildPath);

            // Restore packages using nuget.exe
            var nugetRestoreCommand = string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", solutionPath, msBuildVersionOutput.Output.Trim());
            _logger.LogDebug("Restoring packages using command: {0} {1}", nugetWhereExeResult.Output.Trim(), nugetRestoreCommand);

            try
            {
                var nugetRestoreResult = _processExecutor.Start(solutionDir, nugetWhereExeResult.Output.Trim(), nugetRestoreCommand, timeoutMS: 120000);
                if (nugetRestoreResult.ExitCode != 0)
                {
                    throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.", nugetRestoreResult.Output);
                }
                _logger.LogDebug("Restored packages using nuget.exe, output: {0}", nugetRestoreResult.Output);
            }
            catch (OperationCanceledException)
            {
                throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.");
            }
        }

        #region string helper methods

        private void AppendExampleIfPossible(StringBuilder builder, IEnumerable<string> projectReferences, string filter = null)
        {
            var otherProjectReference = projectReferences.FirstOrDefault(
                o => !string.Equals(o, filter, StringComparison.OrdinalIgnoreCase));
            if (otherProjectReference is null)
            {
                //not possible to find somethig different.
                return;
            }

            builder.AppendLine("");
            builder.AppendLine($"Example: --project-file={otherProjectReference}");
        }

        private string BuildReferenceChoise(IEnumerable<string> projectReferences)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Choose one of the following references:");
            builder.AppendLine("");

            foreach (string projectReference in projectReferences)
            {
                builder.Append("  ");
                builder.AppendLine(projectReference);
            }
            return builder.ToString();
        }

        #endregion
    }
}
