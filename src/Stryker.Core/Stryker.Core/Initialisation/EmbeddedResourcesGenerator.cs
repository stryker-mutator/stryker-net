// Borrowed from https://github.com/Testura/Testura.Mutation/blob/ca2785dba8997ab814be4bb69113739db357810f/src/Testura.Mutation.Core/Execution/Compilation/EmbeddedResourceCreator.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Resources;
using System.Resources.NetStandard;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Stryker.Core.Initialisation
{
    [ExcludeFromCodeCoverage]
    public static class EmbeddedResourcesGenerator
    {
        public static IEnumerable<ResourceDescription> GetManifestResources(string assemblyName, string projectPath, ILogger logger)
        {
            var resources = new List<ResourceDescription>();
            if (!File.Exists(projectPath))
            {
                logger?.LogWarning($"Could not find embedded resources. \n Results may be unreliable if embedded resources are required during test execution.");
                return resources;
            }

            var doc = XDocument.Load(projectPath);
            var rootNamespace = doc.Descendants().FirstOrDefault(d => d.Name.LocalName.Equals("RootNamespace", StringComparison.InvariantCultureIgnoreCase))?.Value;
            var embeddedResources = doc.Descendants().Where(d => d.Name.LocalName.Equals("EmbeddedResource", StringComparison.InvariantCultureIgnoreCase));

            if (rootNamespace == null)
            {
                rootNamespace = assemblyName;
            }

            if (rootNamespace != string.Empty)
            {
                rootNamespace += ".";
            }

            foreach (var embeddedResource in embeddedResources)
            {
                var path = GetEmbeddedPath(embeddedResource);
                if (path == null)
                {
                    continue;
                }

                var resourceFullFilename = Path.Combine(Path.GetDirectoryName(projectPath), path);

                var resourceName =
                    path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ?
                        path.Remove(path.Length - 5) + ".resources" :
                        path;

                resources.Add(new ResourceDescription(
                    $"{rootNamespace}{string.Join(".", resourceName.Split('\\'))}",
                    () => ProvideResourceData(resourceFullFilename),
                    true));
            }


            return resources;
        }

        private static Stream ProvideResourceData(string resourceFullFilename)
        {
            // For non-.resx files just create a FileStream object to read the file as binary data
            if (!resourceFullFilename.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            {
                return File.OpenRead(resourceFullFilename);
            }

            var shortLivedBackingStream = new MemoryStream();
            using (var resourceWriter = new ResourceWriter(shortLivedBackingStream))
            {
                resourceWriter.TypeNameConverter = TypeNameConverter;
                using (var resourceReader = new ResXResourceReader(resourceFullFilename))
                {
                    resourceReader.BasePath = Path.GetDirectoryName(resourceFullFilename);
                    var dictionaryEnumerator = resourceReader.GetEnumerator();
                    while (dictionaryEnumerator.MoveNext())
                    {
                        if (dictionaryEnumerator.Key is string resourceKey)
                        {
                            resourceWriter.AddResource(resourceKey, dictionaryEnumerator.Value);
                        }
                    }
                }
            }

            return new MemoryStream(shortLivedBackingStream.GetBuffer());
        }

        /// <summary>
        /// This is needed to fix a "Could not load file or assembly 'System.Drawing, Version=4.0.0.0"
        /// exception, although I'm not sure why that exception was occurring.
        /// </summary>
        private static string TypeNameConverter(Type objectType) => objectType.AssemblyQualifiedName.Replace("4.0.0.0", "2.0.0.0");

        private static string GetEmbeddedPath(XElement embeddedResource)
        {
            var paths = embeddedResource.Attribute("Include")?.Value;
            if (paths != null)
            {
                return paths;
            }

            paths = embeddedResource.Attribute("Update")?.Value;
            return paths;
        }
    }
}
