using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Compiling
{
    public record CSharpCompilingProcessResult(
        CSharpCompilation Compilation,
        IEnumerable<int> RollbackedIds,
        bool Success);
}
