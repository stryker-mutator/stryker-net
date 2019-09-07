using Stryker.Core.Options;

namespace Stryker.Core.DiffProviders
{
    public interface IDiffProvider
    {
        DiffResult ScanDiff();
    }
}