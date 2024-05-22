using System.Collections.Generic;

namespace Stryker.Core.Compiling
{
    public record CompilingProcessResult(
        bool Success,
        IEnumerable<int> RollbackedIds);
}
