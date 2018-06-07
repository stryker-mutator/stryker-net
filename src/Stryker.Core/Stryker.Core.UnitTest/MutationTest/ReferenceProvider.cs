using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class ReferenceProvider
    {
        public List<PortableExecutableReference> GetReferencedAssemblies()
        {
            var refs = new List<PortableExecutableReference>();
            foreach (var reference in GetTrustedPlatformAssemblyMap())
            {
                refs.Add(MetadataReference.CreateFromFile(reference.Value));
            }
            return refs;
        }


        /// <summary>
        /// This method comes from the RuntimeMetadataReferenceResolver of Roslyn's Scripting API
        /// </summary>
        /// <returns>All references this project has</returns>
        private ImmutableDictionary<string, string> GetTrustedPlatformAssemblyMap()
        {
            var set = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string paths)
            {
                foreach (var path in paths.Split(Path.PathSeparator))
                {
                    if (Path.GetExtension(path) == ".dll")
                    {
                        string fileName = Path.GetFileNameWithoutExtension(path);
                        if (fileName.EndsWith(".ni", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName = fileName.Substring(0, fileName.Length - ".ni".Length);
                        }

                        // last one wins:
                        set[fileName] = path;
                    }
                }
            }

            return set.ToImmutable();
        }
    }
}
