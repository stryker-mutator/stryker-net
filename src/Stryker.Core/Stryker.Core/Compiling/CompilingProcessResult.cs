using System.Collections.Generic;

namespace Stryker.Core.Compiling
{
    public class CompilingProcessResult
    {
        public bool Success { get; set; }
        public IEnumerable<int> RollbackedIds { get; set; }
    }
}
