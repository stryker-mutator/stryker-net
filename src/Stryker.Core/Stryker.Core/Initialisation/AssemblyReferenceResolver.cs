using System.Collections.Generic;
using Microsoft.CodeAnalysis;

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
        private readonly Dictionary<string, AssemblyMetadata> _cache = new();

        public AssemblyReferenceResolver() { }

        /// <summary>
        /// Loads all referenced assemblies
        /// </summary>
        /// <param name="projectReferencePaths">The paths to the assemblies to load</param>
        /// <returns>References</returns>
        public IEnumerable<PortableExecutableReference> LoadProjectReferences(string[] projectReferencePaths)
        {
            foreach (var path in projectReferencePaths)
            {
                yield return GetReference(path);
            }
        }

        private PortableExecutableReference GetReference(string path)
        {
            if (!_cache.ContainsKey(path))
            {
                _cache[path] = AssemblyMetadata.CreateFromFile(path);
            }

            return _cache[path].GetReference();
        }
    }
}
