using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        ProjectAnalyzerResult AnalyzeProject(string path);
        string DetermineProjectUnderTest(IEnumerable<string> projectReferences, string projectUnderTestNameFilter);
        IEnumerable<string> FindSharedProjects(XDocument document);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private const string ErrorMessage = "Project reference issue.";
        private ILogger _logger { get; set; }

        public ProjectFileReader()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public ProjectAnalyzerResult AnalyzeProject(string projectFilePath)
        {
            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            AnalyzerManager manager = new AnalyzerManager();
            var analyzerResult = manager.GetProject(projectFilePath).Build().First();

            return new ProjectAnalyzerResult(analyzerResult);
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
