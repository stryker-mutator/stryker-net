using Stryker.Configuration.Mutants;

namespace Stryker.Configuration.DiffProviders
{
    public interface IDiffProvider
    {
        DiffResult ScanDiff();

        TestSet Tests { get; }
    }
}
