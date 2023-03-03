// Borrowed from https://github.com/Testura/Testura.Mutation/blob/ca2785dba8997ab814be4bb69113739db357810f/src/Testura.Mutation.Core/Execution/Compilation/EmbeddedResourceCreator.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Resources;
using System.Resources.NetStandard;
using Microsoft.CodeAnalysis;
using Mono.Cecil;

namespace Stryker.Core.Initialisation
{
    [ExcludeFromCodeCoverage]
    public static class EmbeddedResourcesGenerator
    {
        private static readonly IDictionary<string, IEnumerable<ResourceDescription>> _resourceDescriptions = new Dictionary<string, IEnumerable<ResourceDescription>>();

        public static IEnumerable<ResourceDescription> GetManifestResources(string assemblyPath, string projectFilePath, string rootNamespace, IEnumerable<string> embeddedResources)
        {
            if (!_resourceDescriptions.ContainsKey(projectFilePath))
            {
                using var module = LoadModule(assemblyPath);
                if (module is not null)
                {
                    _resourceDescriptions.Add(projectFilePath, ReadResourceDescriptionsFromModule(module).ToList());
                }
                else // Failed to load module, generate resources from disk as backup
                {
                    _resourceDescriptions.Add(projectFilePath, GenerateManifestResources(projectFilePath, rootNamespace, embeddedResources));
                }
            }

            foreach (var description in _resourceDescriptions.ContainsKey(projectFilePath) ? _resourceDescriptions[projectFilePath] : Enumerable.Empty<ResourceDescription>())
            {
                yield return description;
            }
        }

        private static ModuleDefinition LoadModule(string assemblyPath)
        {
            try
            {
                return ModuleDefinition.ReadModule(
                    assemblyPath,
                    new ReaderParameters(ReadingMode.Deferred)
                    {
                        InMemory = true,
                        ReadWrite = false,
                        AssemblyResolver = new CrossPlatformAssemblyResolver()
                    });
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<ResourceDescription> ReadResourceDescriptionsFromModule(ModuleDefinition module)
        {
            foreach (var moduleResource in module.Resources.Where(r => r.ResourceType == ResourceType.Embedded).Cast<EmbeddedResource>())
            {
                var stream = moduleResource.GetResourceStream();

                var bytes = new byte[stream.Length];
                _ = stream.Read(bytes, 0, bytes.Length);

                var ms = new MemoryStream();
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;

                yield return new ResourceDescription(
                    moduleResource.Name,
                    () => ms,
                    moduleResource.IsPublic);
            }
        }

        private static IEnumerable<ResourceDescription> GenerateManifestResources(string projectFilePath, string rootNamespace, IEnumerable<string> embeddedResources)
        {
            var resources = new List<ResourceDescription>();
            foreach (var embeddedResource in embeddedResources)
            {
                var resourceFullFilename = Path.Combine(Path.GetDirectoryName(projectFilePath), embeddedResource);

                var resourceName = embeddedResource.Replace("..\\", "");
                resourceName = resourceName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ?
                        resourceName.Remove(0, 1 + resourceName.LastIndexOf("\\")).Replace(".resx", "") + ".resources" :
                        resourceName;

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
                return File.OpenRead(resourceFullFilename);
            }

            var shortLivedBackingStream = new MemoryStream();
            using (var resourceWriter = new ResourceWriter(shortLivedBackingStream))
            {
                resourceWriter.TypeNameConverter = TypeNameConverter;
                using var resourceReader = new ResXResourceReader(resourceFullFilename);
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

            return new MemoryStream(shortLivedBackingStream.GetBuffer());
        }

        /// <summary>
        /// This is needed to fix a "Could not load file or assembly 'System.Drawing, Version=4.0.0.0"
        /// exception, although I'm not sure why that exception was occurring.
        /// </summary>
        private static string TypeNameConverter(Type objectType) => objectType.AssemblyQualifiedName.Replace("4.0.0.0", "2.0.0.0");
    }
}
