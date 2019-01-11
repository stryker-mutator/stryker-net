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
    public class ProjectFileReader
    {
        private const string ErrorMessage = "Project reference issue.";
        private ILogger _logger { get; set; }

        public ProjectFileReader()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public ProjectFile ReadProjectFile(XDocument projectFileContents, string projectUnderTestNameFilter)
        {
            _logger.LogDebug("Reading the project file {0}", projectFileContents.ToString());

            var reference = FindProjectReference(projectFileContents, projectUnderTestNameFilter);
            var targetFramework = FindTargetFrameworkReference(projectFileContents);
            var assemblyName = FindAssemblyName(projectFileContents);
            var appendTargetFrameworkToOutputPath = FindAppendTargetFrameworkToOutputPath(projectFileContents);

            return new ProjectFile()
            {
                ProjectReference = reference,
                TargetFramework = targetFramework,
                AppendTargetFrameworkToOutputPath = appendTargetFrameworkToOutputPath
            };
        }

        public IEnumerable<string> FindSharedProjects(XDocument document)
        {
            var projectReferenceElements = document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "Import", StringComparison.OrdinalIgnoreCase));
            return projectReferenceElements.SelectMany(x => x.Attributes(XName.Get("Project"))).Select(y => FilePathUtils.ConvertPathSeparators(y.Value));
        }

        private string FindProjectReference(XDocument document, string projectUnderTestNameFilter)
        {
            _logger.LogDebug("Determining project under test with name filter {0}", projectUnderTestNameFilter);

            var projectReferenceElements = document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "ProjectReference", StringComparison.OrdinalIgnoreCase));
            // get all the values from the projectReferenceElements
            var projectReferences = projectReferenceElements.SelectMany(x => x.Attributes()).Where(x => string.Equals(x.Name.LocalName, "Include", StringComparison.OrdinalIgnoreCase)).Select(x => x?.Value).ToList();

            if (!projectReferences.Any())
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "No project references found in test project file, unable to find project to mutate.");
            }

            var projectUnderTest = DetermineProjectUnderTest(projectReferences, projectUnderTestNameFilter);
            return FilePathUtils.ConvertPathSeparators(projectUnderTest);
        }

        private string DetermineProjectUnderTest(IEnumerable<string> projectReferences, string projectUnderTestNameFilter)
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

        private string FindTargetFrameworkReference(XDocument document)
        {
            if (document.Elements().Descendants("TargetFrameworks").Any())
            {
                return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "TargetFrameworks", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value.Split(';')[0];
            }
            else
            {
                return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "TargetFramework", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            }
        }

        private bool FindAppendTargetFrameworkToOutputPath(XDocument document)
        {
            if (document.Elements().Descendants("AppendTargetFrameworkToOutputPath").Any())
            {
                return Convert.ToBoolean(document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "AppendTargetFrameworkToOutputPath", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value);
            }

            return true;
        }

        public string FindAssemblyName(XDocument document)
        {
            return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "AssemblyName", StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Value;
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
