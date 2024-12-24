using Stryker.Core.Mutants;

namespace Stryker.Core.DiffProviders;

public interface IDiffProvider
{
    DiffResult ScanDiff();

    TestSet Tests { get; }
}
