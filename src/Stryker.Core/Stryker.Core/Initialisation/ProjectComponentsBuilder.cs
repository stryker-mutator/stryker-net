using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Stryker.Core.Exceptions;
using System.Xml.Linq;
using System.Linq;
using Stryker.Core.ProjectComponents;
using Buildalyzer;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Initialisation
{
    public abstract class ProjectComponentsBuilder
    {
        protected readonly IFileSystem FileSystem;
        public abstract IProjectComponent Build();

        protected ProjectComponentsBuilder(IFileSystem fileSystem) => FileSystem = fileSystem;

        protected IEnumerable<string> ExtractProjectFolders(IAnalyzerResult projectAnalyzerResult)
        {
            var projectFilePath = projectAnalyzerResult.ProjectFilePath;
            var projectFile = FileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var folders = new List<string>();
            var projectDirectory = FileSystem.Path.GetDirectoryName(projectFilePath);
            folders.Add(projectDirectory);

            foreach (var sharedProject in FindSharedProjects(xDocument))
            {
                var sharedProjectName = ReplaceMsbuildProperties(sharedProject, projectAnalyzerResult);

                if (!FileSystem.File.Exists(FileSystem.Path.Combine(projectDirectory, sharedProjectName)))
                {
                    throw new FileNotFoundException($"Missing shared project {sharedProjectName}");
                }

                var directoryName = FileSystem.Path.GetDirectoryName(sharedProjectName);
                folders.Add(FileSystem.Path.Combine(projectDirectory, directoryName));
            }

            return folders;
        }

        private IEnumerable<string> FindSharedProjects(XDocument document)
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

        private static string ReplaceMsbuildProperties(string projectReference, IAnalyzerResult projectAnalyzerResult)
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
                    throw new InputException(message);
                });
        }
    }
}
