using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public static class EmbeddedResourcesGenerator
    {
        private static readonly IDictionary<string, IList<ResourceDescription>> _resourceDescriptions = new Dictionary<string, IList<ResourceDescription>>();

        public static IEnumerable<ResourceDescription> GetManifestResources(string assemblyPath, ILogger logger)
        {
            if (!_resourceDescriptions.ContainsKey(assemblyPath))
            {
                using var module = LoadModule(assemblyPath, logger);
                if (module is null)
                {
                    yield break;
                }
                _resourceDescriptions.Add(assemblyPath, ReadResourceDescriptionsFromModule(module).ToList());
            }

            foreach (var description in _resourceDescriptions[assemblyPath])
            {
                yield return description;
            }
        }

        private static ModuleDefinition LoadModule(string assemblyPath, ILogger logger)
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
            catch (Exception e)
            {
                logger?.LogWarning(e,
                    $"Original project under test {assemblyPath} could not be loaded. \n" +
                    $"Embedded Resources might be missing.");

                return null;
            }
        }

        private static IEnumerable<ResourceDescription> ReadResourceDescriptionsFromModule(ModuleDefinition module)
        {
            foreach (EmbeddedResource moduleResource in module.Resources.Where(r => r.ResourceType == ResourceType.Embedded))
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
    }
}
