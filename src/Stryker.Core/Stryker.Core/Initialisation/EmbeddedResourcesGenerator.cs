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
        public static IEnumerable<ResourceDescription> GetManifestResources(string assemblyPath, ILogger logger)
        {
            try
            {
                var resourceDescriptions = new List<ResourceDescription>();
                using (ModuleDefinition module = ModuleDefinition.ReadModule(
                    assemblyPath,
                    new ReaderParameters(ReadingMode.Immediate)
                    {
                        InMemory = true,
                        ReadWrite = false
                    }))
                {
                    foreach (EmbeddedResource moduleResource in module.Resources.Where(r => r.ResourceType == ResourceType.Embedded))
                    {
                        var ms = new MemoryStream();
                        moduleResource.GetResourceStream().CopyTo(ms);

                        resourceDescriptions.Add(
                            new ResourceDescription(
                                moduleResource.Name,
                                () => ms,
                                moduleResource.IsPublic));
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Original project under test {assemblyPath} could not be loaded. \n" +
                    $"Embedded Resources might be missing.");

                yield break;
            }
        }
    }
}