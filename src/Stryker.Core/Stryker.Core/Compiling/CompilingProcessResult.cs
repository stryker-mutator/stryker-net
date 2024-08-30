using System.Collections.Generic;

namespace Stryker.Abstractions.Compiling
{
    public record CompilingProcessResult(
        bool Success,
        IEnumerable<int> RollbackedIds);
}
