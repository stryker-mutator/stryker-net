using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Compiling
{
    public record CSharpCompilingProcessResult(
        IEnumerable<int> RollbackedIds,
        bool Success);
}
