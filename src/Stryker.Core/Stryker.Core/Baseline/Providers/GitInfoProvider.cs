using System;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Baseline.Providers;

public class GitInfoProvider : IGitInfoProvider
{
    private readonly IStrykerOptions _options;
    private readonly string _repositoryPath;
    private readonly ILogger<GitInfoProvider> _logger;

    public IRepository Repository { get; }

    public string RepositoryPath => _repositoryPath ?? LibGit2Sharp.Repository.Discover(_options.ProjectPath)?.Split(".git")[0];

    public GitInfoProvider(IStrykerOptions options, IRepository repository = null, string repositoryPath = null, ILogger<GitInfoProvider> logger = null)
    {
        _repositoryPath = repositoryPath;
        _options = options;
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<GitInfoProvider>();

        if (!options.Since)
        {
            return;
        }

        Repository = repository ?? CreateRepository();
    }

    public string GetCurrentBranchName()
    {
        string branchName = null;
        if (Repository?.Branches?.FirstOrDefault(b => b.IsCurrentRepositoryHead) is var identifiedBranch && identifiedBranch is { })
        {
            _logger.LogDebug("{BranchName} identified as current branch", identifiedBranch.FriendlyName);
            branchName = identifiedBranch.FriendlyName;
        }

        if (string.IsNullOrWhiteSpace(branchName))
        {
            _logger.LogDebug("Could not locate the current branch name, using project version instead: {ProjectVersion}", _options.ProjectVersion);
            branchName = _options.ProjectVersion;
        }

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new InputException("Unfortunately we could not determine the branch name automatically. Please set the dashboard project version option to your current branch.");
        }
        return branchName;
    }

    public Commit DetermineCommit()
    {
        var commit = GetTargetCommit();

        if (commit == null)
        {
            throw new InputException($"No branch or tag or commit found with given target {_options.SinceTarget}. Please provide a different GitDiffTarget.");
        }

        return commit;
    }

    private IRepository CreateRepository()
    {
        if (string.IsNullOrEmpty(RepositoryPath))
        {
            throw new InputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the 'since' feature.");
        }

        return new Repository(RepositoryPath);
    }

    private Commit GetTargetCommit()
    {
        _logger.LogDebug("Looking for branch matching {gitDiffTarget}", _options.SinceTarget);
        foreach (var branch in Repository.Branches)
        {
            try
            {
                if (branch.UpstreamBranchCanonicalName?.Contains(_options.SinceTarget) ?? false)
                {
                    _logger.LogDebug("Matched with upstream canonical name {upstreamCanonicalName}", branch.UpstreamBranchCanonicalName);
                    return branch.Tip;
                }
                if (branch.CanonicalName?.Contains(_options.SinceTarget) ?? false)
                {
                    _logger.LogDebug("Matched with canonical name {canonicalName}", branch.CanonicalName);
                    return branch.Tip;
                }
                if (branch.FriendlyName?.Contains(_options.SinceTarget) ?? false)
                {
                    _logger.LogDebug("Matched with friendly name {friendlyName}", branch.FriendlyName);
                    return branch.Tip;
                }
            }
            catch (ArgumentNullException)
            {
                // Internal error thrown by libgit2sharp which happens when there is no upstream on a branch.
            }
        }

        _logger.LogDebug("Looking for tag matching {gitDiffTarget}", _options.SinceTarget);
        var tag = Repository.Tags.FirstOrDefault(t => t.Target is Commit && (t.CanonicalName?.Contains(_options.SinceTarget) ?? false));
        var tagCommit = tag?.Target as Commit;
        if (tagCommit != null)
        {
            _logger.LogDebug("Found tag {tag} for diff target {gitDiffTarget}", tag.CanonicalName, _options.SinceTarget);
            return tagCommit;
        }

        // It's a commit!
        if (_options.SinceTarget.Length == 40)
        {
            var commit = Repository.Lookup(new ObjectId(_options.SinceTarget)) as Commit;

            if (commit != null)
            {
                _logger.LogDebug("Found commit {commit} for diff target {gitDiffTarget}", commit.Sha, _options.SinceTarget);
                return commit;
            }
        }

        return null;
    }
}
