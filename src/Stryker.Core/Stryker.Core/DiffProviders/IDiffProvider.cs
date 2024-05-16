using Stryker.Shared.Tests;

namespace Stryker.Core.DiffProviders;

public interface IDiffProvider
{
    DiffResult ScanDiff();

    ITestSet Tests { get; }
}
