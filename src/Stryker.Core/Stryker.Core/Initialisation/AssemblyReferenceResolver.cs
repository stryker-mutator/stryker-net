using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation
{
    public interface IAssemblyReferenceResolver
    {
        IEnumerable<PortableExecutableReference> LoadProjectReferences(string[] projectReferencePaths);
    }

    /// <summary>
    /// Resolving the MetadataReferences for compiling later
    /// This has to be done using msbuild because currently msbuild is the only reliable way of resolving all referenced assembly locations
    /// </summary>
    public class AssemblyReferenceResolver : IAssemblyReferenceResolver
    {
        private readonly ILogger _logger;

        public AssemblyReferenceResolver()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AssemblyReferenceResolver>();
        }

        /// <summary>
        /// Uses Buildalyzer to resolve all references for the given test project
        /// </summary>
        /// <param name="projectFile">The test project file location</param>
        /// <returns>References</returns>
        public IEnumerable<PortableExecutableReference> LoadProjectReferences(string[] projectReferencePaths)
        {
            foreach (var path in projectReferencePaths)
            {
                _logger.LogTrace("Resolved dependency {0}", path);
                yield return MetadataReference.CreateFromFile(path);
            }
        }
    }
}
