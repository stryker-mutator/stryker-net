using System.Collections.Generic;

namespace Stryker.Core.DiffProviders
{
    public class DiffResult
    {
        public bool TestsChanged { get; set; }
        public ICollection<string> ChangedFiles { get; set; }
    }
}