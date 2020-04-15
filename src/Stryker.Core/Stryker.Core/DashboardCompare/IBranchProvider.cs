namespace Stryker.Core.DashboardCompare
{
    public interface IBranchProvider
    {
        string GetCurrentBranchCanonicalName();
    }
}
