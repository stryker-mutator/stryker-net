using LibGit2Sharp;

namespace Stryker.Configuration.Baseline.Providers
{
    public interface IGitInfoProvider
    {
        IRepository Repository { get; }

        string RepositoryPath { get; }

        string GetCurrentBranchName();

        Commit DetermineCommit();
    }
}
