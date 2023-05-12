namespace Stryker.Core.Baseline.Providers;
using LibGit2Sharp;

public interface IGitInfoProvider
{
    IRepository Repository { get; }

    string RepositoryPath { get; }

    string GetCurrentBranchName();

    Commit DetermineCommit();
}
