namespace Stryker.Core.DiffProviders
{
    public interface IDiffProvider
    {
        DiffResult ScanDiff();
    }
}
