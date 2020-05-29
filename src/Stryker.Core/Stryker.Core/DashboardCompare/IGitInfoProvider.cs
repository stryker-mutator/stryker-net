using LibGit2Sharp;

namespace Stryker.Core.DashboardCompare
{
    public interface IGitInfoProvider
    {
        IRepository Repository { get; }

        string RepositoryPath { get; }

        string GetCurrentBranchName();

        Commit DetermineCommit();
    }
}
