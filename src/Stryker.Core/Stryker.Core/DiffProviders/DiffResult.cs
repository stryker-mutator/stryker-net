namespace Stryker.Core.DiffProviders;
using System.Collections.Generic;

public class DiffResult
{
    public ICollection<string> ChangedTestFiles { get; set; }
    public ICollection<string> ChangedSourceFiles { get; set; }
}