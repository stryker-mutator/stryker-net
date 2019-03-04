using Buildalyzer;
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
        public static IEnumerable<ResourceDescription> GetManifestResources(AnalyzerResult analyzerResult, ILogger logger)
        {
            var originalDllFile = Path.Combine(
                Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                analyzerResult.Items["IntermediateAssembly"][0].ItemSpec);

            ModuleDefinition module;

            try
            {
                module = ModuleDefinition.ReadModule(originalDllFile);
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Original project under test {originalDllFile} could not be loaded. \n" +
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
        }
    }
}