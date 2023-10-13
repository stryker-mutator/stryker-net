using System.Collections.Generic;

namespace Stryker.Core.Compiling
{
    public record CSharpCompilingProcessResult(
        bool Success,
        IEnumerable<int> RollbackedIds);
}
