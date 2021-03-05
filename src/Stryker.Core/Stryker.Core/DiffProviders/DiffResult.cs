using System.Collections.Generic;

namespace Stryker.Core.DiffProviders
{
    public class DiffResult
    {
        public ICollection<ChangedFile> ChangedTestFiles { get; set; }
        public ICollection<ChangedFile> ChangedSourceFiles { get; set; }
    }
}
