using Stryker.Abstractions.Testing;

namespace Stryker.Core.DiffProviders;

public interface IDiffProvider
{
    DiffResult ScanDiff();

    ITestSet Tests { get; }
}
