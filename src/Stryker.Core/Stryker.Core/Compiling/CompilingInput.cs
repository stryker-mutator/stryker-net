using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Stryker.Core.Compiling
{
    public class CompilingInput
    {
        public string AssemblyName { get; set; }
        public IEnumerable<PortableExecutableReference> References { get; set; }
    }
}
