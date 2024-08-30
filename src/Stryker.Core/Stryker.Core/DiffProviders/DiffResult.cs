using System.Collections.Generic;

namespace Stryker.Abstractions.DiffProviders
{
    public class DiffResult
    {
        public ICollection<string> ChangedTestFiles { get; set; }
        public ICollection<string> ChangedSourceFiles { get; set; }
    }
}