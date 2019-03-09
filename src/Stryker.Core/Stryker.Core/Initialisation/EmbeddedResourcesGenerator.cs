using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public static class EmbeddedResourcesGenerator
    {
        public static IEnumerable<ResourceDescription> GetManifestResources(string assemblyPath, ILogger logger)
        {
            ModuleDefinition module;

            try
            {
                module = ModuleDefinition.ReadModule(assemblyPath);
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Original project under test {assemblyPath} could not be loaded. \n" +
                    $"Embedded Resources might be missing.");

                yield break;
            }

            foreach (EmbeddedResource moduleResource in module.Resources.Where(r => r.ResourceType == ResourceType.Embedded))
            {
                yield return new ResourceDescription(
                    moduleResource.Name,
                    () => moduleResource.GetResourceStream(),
                    moduleResource.IsPublic);
            }
            module.Dispose();
        }
    }
}