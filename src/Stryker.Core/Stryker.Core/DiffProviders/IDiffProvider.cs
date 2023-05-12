namespace Stryker.Core.DiffProviders;
using Stryker.Core.Mutants;

public interface IDiffProvider
{
    DiffResult ScanDiff();

    TestSet Tests { get; }
}
