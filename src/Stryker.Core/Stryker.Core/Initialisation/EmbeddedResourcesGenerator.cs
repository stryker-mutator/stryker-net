// Borrowed from https://github.com/Testura/Testura.Mutation/blob/ca2785dba8997ab814be4bb69113739db357810f/src/Testura.Mutation.Core/Execution/Compilation/EmbeddedResourceCreator.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Resources;
using System.Resources.NetStandard;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Initialisation
{
    [ExcludeFromCodeCoverage]
    public static class EmbeddedResourcesGenerator
    {
        public static IEnumerable<ResourceDescription> GetManifestResources(string projectFilePath, string rootNamespace, IEnumerable<string> embeddedResources)
        {
            var resources = new List<ResourceDescription>();

            foreach (var embeddedResource in embeddedResources)
            {
                var resourceFullFilename = Path.Combine(Path.GetDirectoryName(projectFilePath), embeddedResource);

                var resourceName =
                    embeddedResource.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ?
                        embeddedResource.Remove(embeddedResource.Length - 5) + ".resources" :
                        embeddedResource;

                resources.Add(new ResourceDescription(
                    $"{rootNamespace}.{string.Join(".", resourceName.Split('\\'))}",
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
                
                if (!File.Exists(resourceFullFilename))
                {
                    return new MemoryStream();
                }
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
    }
}
