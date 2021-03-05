using LibGit2Sharp;
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
        private readonly IStrykerOptions _options;
        private readonly string _repositoryPath;
        private readonly ILogger<GitInfoProvider> _logger;

        public IRepository Repository { get; }

        public string RepositoryPath => _repositoryPath ?? LibGit2Sharp.Repository.Discover(_options.BasePath)?.Split(".git")[0];

        public GitInfoProvider(IStrykerOptions options, IRepository repository = null, string repositoryPath = null, ILogger<GitInfoProvider> logger = null)
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
            var commit = GetTargetCommit();

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given target {_options.GitDiffSource}. Please provide a different GitDiffTarget.");
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

        private Commit GetTargetCommit()
        {
            Branch targetBranch = null;

            _logger.LogDebug("Looking for branch matching {gitDiffTarget}", _options.GitDiffSource);
            foreach (var branch in Repository.Branches)
            {
                try
                {
                    if (branch.UpstreamBranchCanonicalName?.Contains(_options.GitDiffSource) ?? false)
                    {
                        _logger.LogDebug("Matched with upstream canonical name {upstreamCanonicalName}", branch.UpstreamBranchCanonicalName);
                        targetBranch = branch;
                        break;
                    }
                    if (branch.CanonicalName?.Contains(_options.GitDiffSource) ?? false)
                    {
                        _logger.LogDebug("Matched with canonical name {canonicalName}", branch.CanonicalName);
                        targetBranch = branch;
                        break;
                    }
                    if (branch.FriendlyName?.Contains(_options.GitDiffSource) ?? false)
                    {
                        _logger.LogDebug("Matched with friendly name {friendlyName}", branch.FriendlyName);
                        targetBranch = branch;
                        break;
                    }
                }
                catch (ArgumentNullException)
                {
                    // Internal error thrown by libgit2sharp which happens when there is no upstream on a branch.
                }
            }

            if (targetBranch != null)
            {
                return targetBranch.Tip;
            }

            // It's a commit!
            if (_options.GitDiffSource.Length == 40)
            {
                var commit = Repository.Lookup(new ObjectId(_options.GitDiffSource)) as Commit;

                if (commit != null)
                {
                    _logger.LogDebug($"Found commit {commit.Sha} for diff target {_options.GitDiffSource}");
                    return commit;
                }
            }

            return null;
        }
    }
}
