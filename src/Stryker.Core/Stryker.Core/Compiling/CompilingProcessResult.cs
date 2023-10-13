using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Compiling
{
    public class CompilingProcessResult
    {
        public bool Success { get; set; }
        public CSharpCompilation Compilation { get; set; }
        public IEnumerable<int> RollbackedIds { get; set; }
    }
}
