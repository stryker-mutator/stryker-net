using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public class ProjectFileReader
    {
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

            return new ProjectFile()
            {
                ProjectReference = reference,
                TargetFramework = targetFramework
            };
        }

        private string FindProjectReference(XDocument document, string projectUnderTestNameFilter)
        {
            _logger.LogDebug("Determining project under test with name filter {0}", projectUnderTestNameFilter);

            var projectReferenceElements = document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "ProjectReference", StringComparison.OrdinalIgnoreCase));
            // get all the values from the projectReferenceElements
            var projectReferences = projectReferenceElements.SelectMany(x => x.Attributes()).Where(x => string.Equals(x.Name.LocalName, "Include", StringComparison.OrdinalIgnoreCase)).Select(x => x?.Value).ToList();

            if (projectReferences.Count() > 1)
            {
                // put the references together in one string seperated by ", "
                string referencesString = string.Join(", ", projectReferences);
                string referenceChoise =
                    $"{Environment.NewLine}Choose one of the following references:{Environment.NewLine} {referencesString.Replace(",", Environment.NewLine)}";
                if (string.IsNullOrEmpty(projectUnderTestNameFilter))
                {
                    throw new StrykerInputException(
                        $"Test project contains more than one project references. Please add the --project-file=[projectname] argument to specify whitch project to mutate. {referenceChoise}");
                }
                else
                {
                    var searchResult = projectReferences.Where(x => x.ToLower().Contains(projectUnderTestNameFilter.ToLower())).ToList();
                    if (!searchResult.Any())
                    {
                        throw new StrykerInputException(
                            $"No project reference matched your --project-file={projectUnderTestNameFilter} argument to specify the project to mutate, was the name spelled correctly? {referenceChoise}.");
                    }
                    else if (searchResult.Count() > 1)
                    {
                        throw new StrykerInputException(
                            $"More than one project reference matched your --project-file={projectUnderTestNameFilter} argument to specify the project to mutate, please specify the name more detailed. {referenceChoise}.)");
                    }
                    return FilePathUtils.ConvertPathSeparators(searchResult.Single());
                }
            }
            else if (!projectReferences.Any())
            {
                throw new StrykerInputException("No project references found in test project file, unable to find project to mutate.");
            }
            return FilePathUtils.ConvertPathSeparators(projectReferences.Single());
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

        public string FindAssemblyName(XDocument document)
        {
            return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "AssemblyName", StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Value;
        }
    }
}
