using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;

namespace Stryker.Core.DashboardCompare
{
    using System.Linq;

    using Logging;

    using Microsoft.Extensions.Logging;

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

        public GitInfoProvider(StrykerOptions options, IRepository repository = null, ILogger<GitInfoProvider> logger = null, string repositoryPath = null)
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
            return Repository?.Branches?.FirstOrDefault(x => x.IsCurrentRepositoryHead)?.FriendlyName ?? string.Empty;
        }

        private IRepository CreateRepository()
        {
            if (string.IsNullOrEmpty(RepositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }

            return new Repository(RepositoryPath);
        }

        private void Checkout(GitSourceKinds gitSourceKind)
        {
            try
            {
                var branchName = gitSourceKind == GitSourceKinds.FriendlyBranchName ? _options.GitSource : GetFriendlyName(_options.GitSource);

                _logger.LogDebug($"Creating branch ${branchName} with committish origin/{branchName}");
                var branch = Repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");
                _logger.LogDebug($"Checking out branch ${branchName}");
                Commands.Checkout(Repository, branch);

                _logger.LogDebug($"Creating branch ${_options.ProjectVersion} with committish origin/{_options.ProjectVersion}");
                var currentBranch = Repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");
                _logger.LogDebug($"Checking out branch ${_options.ProjectVersion}");
                Commands.Checkout(Repository, currentBranch);
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Something went wrong during checkout.\n{e.Message}");
                // Do nothing, Checkout is already done
            }
        }

        private string GetFriendlyName(string canonicalBranchName)
        {
            return string.Join('/', canonicalBranchName.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(2));
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
    }
}
