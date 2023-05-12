// Partially borrowed from https://github.com/Testura/Testura.Mutation/blob/ca2785dba8997ab814be4bb69113739db357810f/src/Testura.Mutation.Core/Execution/Compilation/EmbeddedResourceCreator.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
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

                // Failed to load some or all resources from module, generate missing resources from disk
                if (module is not null && _resourceDescriptions[projectFilePath].Count() < embeddedResources.Count())
                {
                    var missingEmbeddedResources = embeddedResources.Where(r => _resourceDescriptions[projectFilePath].Any(fr => GetResourceDescriptionInternalName(fr) == GenerateResourceName(r)));
                    _resourceDescriptions[projectFilePath] = _resourceDescriptions[projectFilePath].Concat(GenerateManifestResources(projectFilePath, rootNamespace, missingEmbeddedResources));
                }

                // Failed to load module, generate all resources from disk
                if (module is null)
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
                var shortLivedBackingStream = moduleResource.GetResourceStream();

                var resourceStream = new MemoryStream();
                shortLivedBackingStream.CopyTo(resourceStream);

                // reset streams back to start
                resourceStream.Position = 0;
                shortLivedBackingStream.Position = 0;

                yield return new ResourceDescription(
                    moduleResource.Name,
                    () => resourceStream,
                    moduleResource.IsPublic);
            }
        }

        private static IEnumerable<ResourceDescription> GenerateManifestResources(string projectFilePath, string rootNamespace, IEnumerable<string> embeddedResources)
        {
            var resources = new List<ResourceDescription>();
            foreach (var embeddedResource in embeddedResources)
            {
                var resourceFullFilename = Path.Combine(Path.GetDirectoryName(projectFilePath), embeddedResource);

                var resourceName = GenerateResourceName(embeddedResource);

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

        private static string GenerateResourceName(string filePath)
        {
            // Remove relative path sequences
            var resourceName = filePath.Replace("..\\", "");

            // If the resource is a resx file, take the file name and replace the extension with 'resources', otherwise return full resource name
            return resourceName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ?
                    resourceName.Remove(0, 1 + resourceName.LastIndexOf("\\")).Replace(".resx", "") + ".resources" : filePath;
        }

        private static string GetResourceDescriptionInternalName(ResourceDescription resource) =>
            (string)typeof(ResourceDescription).GetField("ResourceName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(resource);

    }
}
