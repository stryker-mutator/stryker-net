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
        private enum GitSourceKinds
        {
            Unknown,
            Commit,
            CanonicalBranchName,
            FriendlyBranchName
        }

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
            if (Repository?.Branches == null)
            {
                _logger.LogInformation("There is no information available about your current branch. Performing a checkout.");
                //Checkout();
            }

            if (Repository?.Branches != null)
            {
                foreach (var branch in Repository.Branches)
                {
                    if (branch.IsCurrentRepositoryHead)
                    {
                        _logger.LogInformation("{0} identified as current branch", branch.FriendlyName);
                        return branch.FriendlyName;
                    }
                }
            }

            _logger.LogInformation("Could not locate the current branch name");
            return null;
        }

        public Commit DetermineCommit()
        {
            var (gitSourceKind, commit) = GetCommit();

            if (commit == null)
            {
                Checkout(gitSourceKind);
                (_, commit) = GetCommit();
            }

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given source {_options.GitSource}. Please provide a different --git-source or remove this option.");
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

        private (GitSourceKinds, Commit) GetCommit()
        {
            GitSourceKinds gitSourceKind = GitSourceKinds.Unknown;
            Branch sourceBranch = null;
            foreach (var branch in Repository.Branches)
            {
                try
                {
                    if (branch.CanonicalName == _options.GitSource)
                    {
                        sourceBranch = branch;
                        gitSourceKind = GitSourceKinds.CanonicalBranchName;
                        break;
                    }
                    if (branch.FriendlyName == _options.GitSource)
                    {
                        sourceBranch = branch;
                        gitSourceKind = GitSourceKinds.FriendlyBranchName;
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
                return (gitSourceKind, sourceBranch.Tip);
            }

            if (_options.GitSource.Length == 40)
            {
                var commit = Repository.Lookup(new ObjectId(_options.GitSource)) as Commit;

                if (commit != null)
                {
                    _logger.LogDebug($"Found commit {commit.Sha} for commit {_options.GitSource}");
                    return (GitSourceKinds.Commit, commit);
                }
            }

            return (gitSourceKind, null);
        }

        private void Checkout(GitSourceKinds gitSourceKind)
        {
            try
            {
                var currentCommit = Repository.Head.Tip;
                var branchName = gitSourceKind == GitSourceKinds.FriendlyBranchName ? _options.GitSource : GetFriendlyName(_options.GitSource);

                _logger.LogDebug($"Creating branch ${branchName} with committish origin/{branchName}");
                var branch = Repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");
                _logger.LogDebug($"Checking out branch ${branchName}");
                Commands.Checkout(Repository, branch);

                _logger.LogDebug($"Checking out cached commit ${currentCommit.Sha}");
                Commands.Checkout(Repository, currentCommit);
            }
            catch
            {
                // Do nothing, Checkout is already done
            }
        }

        private string GetFriendlyName(string canonicalBranchName)
        {
            return string.Join('/', canonicalBranchName.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(2));
        }
    }
}
