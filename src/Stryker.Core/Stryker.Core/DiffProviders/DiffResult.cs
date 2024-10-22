using System.Collections.Generic;

namespace Stryker.Core.DiffProviders;

public class DiffResult
{
    public ICollection<string> ChangedTestFiles { get; set; }
    public ICollection<string> ChangedSourceFiles { get; set; }
}
