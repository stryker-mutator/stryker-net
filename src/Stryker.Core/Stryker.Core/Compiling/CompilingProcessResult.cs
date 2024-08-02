using System.Collections.Generic;

namespace Stryker.Configuration.Compiling
{
    public record CompilingProcessResult(
        bool Success,
        IEnumerable<int> RollbackedIds);
}
