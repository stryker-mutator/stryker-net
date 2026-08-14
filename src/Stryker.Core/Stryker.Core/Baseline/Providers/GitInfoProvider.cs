using System;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Baseline.Providers;

public class GitInfoProvider : IGitInfoProvider
{
    private readonly IStrykerOptions _options;
    private readonly string _repositoryPath;
    private readonly ILogger<GitInfoProvider> _logger;

    public bool IsRepository { get; }

    public IRepository Repository { get; }

    public string RepositoryPath => _repositoryPath;

    public GitInfoProvider(IStrykerOptions options, IRepository repository = null, string repositoryPath = null, ILogger<GitInfoProvider> logger = null)
    {
        _repositoryPath = repositoryPath;
        _options = options;
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<GitInfoProvider>();

        _repositoryPath ??= DiscoverRepositoryPath();
        Repository = repository ?? (string.IsNullOrEmpty(_repositoryPath) ? null : new Repository(_repositoryPath));
        IsRepository = Repository is not null;
    }

    public string GetCurrentBranchName()
    {
        if (!IsRepository)
        {
            _logger.LogDebug("Could not locate a git repository, unable to determine the current branch name");
            return null;
        }

        string branchName = null;
        if (Repository?.Branches?.FirstOrDefault(b => b.IsCurrentRepositoryHead) is var identifiedBranch && identifiedBranch is not null)
        {
            _logger.LogDebug("{BranchName} identified as current branch", identifiedBranch.FriendlyName);
            branchName = identifiedBranch.FriendlyName;
        }

        return branchName;
    }

    public Commit DetermineCommit(string target)
    {
        if (!IsRepository)
        {
            return null;
        }

        var commit = GetCommit(target);

        return commit;
    }

    private string DiscoverRepositoryPath()
    {
        if (string.IsNullOrWhiteSpace(_options.ProjectPath))
        {
            return null;
        }

        return LibGit2Sharp.Repository.Discover(_options.ProjectPath)?.Split(".git")[0];
    }

    private Commit GetCommit(string target)
    {
        _logger.LogDebug("Looking for branch matching {Target}", target);
        foreach (var branch in Repository.Branches)
        {
            try
            {
                if (branch.UpstreamBranchCanonicalName?.Contains(target) ?? false)
                {
                    _logger.LogDebug("Matched with upstream canonical name {UpstreamCanonicalName}", branch.UpstreamBranchCanonicalName);
                    return branch.Tip;
                }
                if (branch.CanonicalName?.Contains(target) ?? false)
                {
                    _logger.LogDebug("Matched with canonical name {CanonicalName}", branch.CanonicalName);
                    return branch.Tip;
                }
                if (branch.FriendlyName?.Contains(target) ?? false)
                {
                    _logger.LogDebug("Matched with friendly name {FriendlyName}", branch.FriendlyName);
                    return branch.Tip;
                }
            }
            catch (ArgumentNullException)
            {
                // Internal error thrown by libgit2sharp which happens when there is no upstream on a branch.
            }
        }

        _logger.LogDebug("Looking for tag matching {Target}", target);
        var tag = Repository.Tags.FirstOrDefault(t => t.Target is Commit && (t.CanonicalName?.Contains(target) ?? false));
        var tagCommit = tag?.Target as Commit;
        if (tagCommit != null)
        {
            _logger.LogDebug("Found tag {Tag} for diff target {Target}", tag.CanonicalName, target);
            return tagCommit;
        }

        // It's a commit!
        if (target.Length == 40)
        {
            var commit = Repository.Lookup(new ObjectId(target)) as Commit;

            if (commit != null)
            {
                _logger.LogDebug("Found commit {Commit} for diff target {Target}", commit.Sha, target);
                return commit;
            }
        }

        return null;
    }
}
