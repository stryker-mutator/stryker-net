using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Abstractions.Compiling
{
    public record CSharpRollbackProcessResult(
        CSharpCompilation Compilation,
        IEnumerable<int> RollbackedIds);
}
