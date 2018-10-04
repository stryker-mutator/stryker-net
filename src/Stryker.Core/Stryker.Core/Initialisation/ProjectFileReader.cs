using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public class ProjectFileReader {
        private ILogger _logger { get; set; }

        public ProjectFileReader () {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader> ();
        }

        public ProjectFile ReadProjectFile (XDocument projectFileContents, string projectUnderTestNameFilter) {
            _logger.LogDebug ("Reading the project file {0}", projectFileContents.ToString ());

            var rererence = FindProjectReference (projectFileContents, projectUnderTestNameFilter);
            var targetFramework = FindTargetFrameworkReference (projectFileContents);
            var assemblyName = FindAssemblyName (projectFileContents);

            return new ProjectFile () {
                ProjectReference = rererence,
                    TargetFramework = targetFramework
            };
        }

        private string FindProjectReference (XDocument document, string projectUnderTestNameFilter) {
            _logger.LogDebug ("Determining project under test with name filter {0}", projectUnderTestNameFilter);

            var projectReferenceElements = document.Elements ().Descendants ().Where (x => string.Equals (x.Name.LocalName, "ProjectReference", StringComparison.OrdinalIgnoreCase));
            // get all the values from the projectReferenceElements
            var projectReferences = projectReferenceElements.SelectMany (x => x.Attributes ()).Where (x => string.Equals (x.Name.LocalName, "Include", StringComparison.OrdinalIgnoreCase)).Select (x => x?.Value).ToList ();

            if (projectReferences.Count () > 1) {
                // put the references together in one string seperated by ", "
                string referencesString = string.Join (", ", projectReferences);
                if (string.IsNullOrEmpty (projectUnderTestNameFilter)) {
                    throw new NotSupportedException ("Only one referenced project is supported, please add the --project-file=[projectname] argument to specify the project to mutate", innerException : new Exception ($"Found the following references: {referencesString}"));
                } else {
                    var searchResult = projectReferences.Where (x => x.ToLower ().Contains (projectUnderTestNameFilter.ToLower ())).ToList ();
                    if (!searchResult.Any ()) {
                        throw new ArgumentException ($"No project reference matched your --project-file={projectUnderTestNameFilter} argument to specify the project to mutate, was the name spelled correctly?", innerException : new Exception ($"Found the following references: {referencesString}"));
                    } else if (searchResult.Count () > 1) {
                        throw new ArgumentException ($"More than one project reference matched your --project-file={projectUnderTestNameFilter} argument to specify the project to mutate, please specify the name more detailed", innerException : new Exception ($"Found the following references: {referencesString}"));
                    }
                    return searchResult.Single ();
                }
            } else if (!projectReferences.Any ()) {
                throw new NotSupportedException ("No project references found in test project file, unable to find project to mutate.");
            }
            return projectReferences.Single ();

        }

        private string FindTargetFrameworkReference (XDocument document) {
            if (document.Elements().Descendants("TargetFrameworks").Any())
            {
                return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "TargetFrameworks", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value.Split(';')[0];
            }
            else
            {
                return document.Elements().Descendants().Where(x => string.Equals(x.Name.LocalName, "TargetFramework", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            }
        }

        public string FindAssemblyName (XDocument document) {
            return document.Elements ().Descendants ().Where (x => string.Equals (x.Name.LocalName, "AssemblyName", StringComparison.OrdinalIgnoreCase)).FirstOrDefault ()?.Value;
        }
    }
}