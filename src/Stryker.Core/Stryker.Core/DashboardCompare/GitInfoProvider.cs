﻿using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;

namespace Stryker.Core.DashboardCompare
{
    using Logging;
    using Microsoft.Extensions.Logging;
    using System.Linq;

    public class GitInfoProvider : IGitInfoProvider
    {
        private readonly StrykerOptions _options;
        private readonly string _repositoryPath;
        private readonly ILogger<GitInfoProvider> _logger;

        public IRepository Repository { get; }

        public string RepositoryPath => _repositoryPath ?? LibGit2Sharp.Repository.Discover(_options.BasePath)?.Split(".git")[0];

        public GitInfoProvider(StrykerOptions options, IRepository repository = null, string repositoryPath = null, ILogger<GitInfoProvider> logger = null)
        {
            _repositoryPath = repositoryPath;
            _options = options;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<GitInfoProvider>();

            if (!options.DiffEnabled)
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
                _logger.LogDebug("{0} identified as current branch", identifiedBranch.FriendlyName);
                branchName = identifiedBranch.FriendlyName;
            }

            if (string.IsNullOrWhiteSpace(branchName))
            {
                _logger.LogDebug("Could not locate the current branch name, using project version instead: {0}", _options.ProjectVersion);
                branchName = _options.ProjectVersion;
            }

            if (string.IsNullOrWhiteSpace(branchName))
            {
                throw new StrykerInputException("Unfortunately we could not determine the branch name automatically. Please set the dashboard project version option to your current branch.");
            }
            return branchName;
        }

        public Commit DetermineCommit()
        {
            var commit = GetCommit();

            if (commit == null)
            {
                CreateLocalBranchForGitDiffTarget();
                commit = GetCommit();
            }

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given target {_options.GitDiffTarget}. Please provide a different GitDiffTarget.");
            }

            return commit;
        }

        private IRepository CreateRepository()
        {
            if (string.IsNullOrEmpty(RepositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }

            return new Repository(RepositoryPath);
        }

        private Commit GetCommit()
        {
            Branch sourceBranch = null;
            foreach (var branch in Repository.Branches)
            {
                _logger.LogInformation("Found canonical branch: {CanonicalName}", branch.CanonicalName);
                _logger.LogInformation("With friendly name: {FriendlyName}", branch.FriendlyName);
                _logger.LogInformation("With remote name: {RemoteName}", branch.RemoteName);
                _logger.LogInformation("With upstream branch canonical name: {UpstreamBranchCanonicalName}", branch.UpstreamBranchCanonicalName);
                try
                {
                    if (branch.UpstreamBranchCanonicalName.Contains(_options.GitDiffTarget))
                    {
                        sourceBranch = branch;
                        break;
                    }
                    if (branch.CanonicalName.Contains(_options.GitDiffTarget))
                    {
                        sourceBranch = branch;
                        break;
                    }
                    if (branch.FriendlyName.Contains(_options.GitDiffTarget))
                    {
                        sourceBranch = branch;
                        break;
                    }
                }
                catch (ArgumentNullException)
                {
                    // Internal error thrown by libgit2sharp which happens when there is no upstream on a branch.
                }
            }

            if (sourceBranch != null)
            {
                return sourceBranch.Tip;
            }

            // It's a commit!
            if (_options.GitDiffTarget.Length == 40)
            {
                var commit = Repository.Lookup(new ObjectId(_options.GitDiffTarget)) as Commit;

                if (commit != null)
                {
                    _logger.LogDebug($"Found commit {commit.Sha} for branch {_options.GitDiffTarget}");
                    return commit;
                }
            }

            return null;
        }

        private void CreateLocalBranchForGitDiffTarget()
        {
            try
            {
                var currentCommit = Repository.Head.Tip;

                _logger.LogDebug("Creating branch {0} with committish dashboard-compare/{0}", _options.GitDiffTarget);
                var targetBranch = Repository.CreateBranch(_options.GitDiffTarget, $"dashboard-compare/{_options.GitDiffTarget}");
                _logger.LogDebug("Checking out branch {0}", _options.GitDiffTarget);
                Commands.Checkout(Repository, targetBranch);

                _logger.LogDebug("Checking out cached commit {0}", currentCommit.Sha);
                Commands.Checkout(Repository, currentCommit);
            }
            catch
            {
                // Do nothing, Checkout is already done
            }
        }
    }
}
