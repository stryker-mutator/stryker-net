using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Stryker.Core.Exceptions;
using Stryker.Core.InjectedHelpers;
using System.Xml.Linq;
using System.Linq;
using Stryker.Core.ProjectComponents;
using Buildalyzer;

namespace Stryker.Core.Initialisation
{
    public abstract class ProjectComponentsBuilder
    {
        public abstract IProjectComponent Build();

        protected IEnumerable<string> FindSharedProjects(XDocument document)
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

        protected static string ReplaceMsbuildProperties(string projectReference, IAnalyzerResult projectAnalyzerResult)
        {
            var propertyRegex = new Regex(@"\$\(([a-zA-Z_][a-zA-Z0-9_\-.]*)\)");
            var properties = projectAnalyzerResult.Properties;

            return propertyRegex.Replace(projectReference,
                m =>
                {
                    var property = m.Groups[1].Value;
                    if (properties.TryGetValue(property, out var propertyValue))
                    {
                        return propertyValue;
                    }

                    var message = $"Missing MSBuild property ({property}) in project reference ({projectReference}). Please check your project file ({projectAnalyzerResult.ProjectFilePath}) and try again.";
                    throw new StrykerInputException(message);
                });
        }
    }
}
