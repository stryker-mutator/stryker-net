using System.Collections.Generic;

namespace Stryker.Core.DiffProviders;

public class DiffResult
{
    public IDictionary<string, List<int>> ChangedTestFiles { get; set; }
    public IDictionary<string, List<int>> ChangedSourceFiles { get; set; }
}
