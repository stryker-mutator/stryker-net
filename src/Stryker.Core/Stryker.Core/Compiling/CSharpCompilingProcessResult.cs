using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Stryker.Core.Compiling
{
    public record CSharpCompilingProcessResult(
        CSharpCompilation Compilation,
        IEnumerable<int> RollbackedIds,
        EmitResult EmitResult);
}
