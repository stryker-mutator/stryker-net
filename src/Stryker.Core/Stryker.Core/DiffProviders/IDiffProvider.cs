using Stryker.Abstractions.Mutants;

namespace Stryker.Abstractions.DiffProviders
{
    public interface IDiffProvider
    {
        DiffResult ScanDiff();

        TestSet Tests { get; }
    }
}
