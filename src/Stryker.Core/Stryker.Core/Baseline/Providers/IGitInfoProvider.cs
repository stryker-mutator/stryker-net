using LibGit2Sharp;

namespace Stryker.Core.Baseline.Providers;

public interface IGitInfoProvider
{
    IRepository Repository { get; }

    string RepositoryPath { get; }

    string GetCurrentBranchName();

    Commit DetermineCommit();
}
