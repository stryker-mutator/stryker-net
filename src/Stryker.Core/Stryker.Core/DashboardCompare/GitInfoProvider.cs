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

        public string RepositoryPath => this._repositoryPath ?? LibGit2Sharp.Repository.Discover(this._options.BasePath)?.Split(".git")[0];

        public GitInfoProvider(StrykerOptions options, IRepository repository = null, ILogger<GitInfoProvider> logger = null, string repositoryPath = null)
        {
            this._repositoryPath = repositoryPath;
            this._options = options;
            this._logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<GitInfoProvider>();
            
            if (!options.DiffEnabled)
            {
                return;
            }

            this.Repository = repository ?? this.CreateRepository();
        }

        public string GetCurrentBranchName()
        {
            return this.Repository?.Branches?.FirstOrDefault(x => x.IsCurrentRepositoryHead)?.FriendlyName ?? string.Empty;
        }

        private IRepository CreateRepository()
        {
            if (string.IsNullOrEmpty(this.RepositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }

            return new Repository(this.RepositoryPath);
        }

        private void Checkout(GitSourceKinds gitSourceKind)
        {
            try
            {
                var branchName = gitSourceKind == GitSourceKinds.FriendlyBranchName ? this._options.GitSource : this.GetFriendlyName(this._options.GitSource);

                this._logger.LogDebug($"Creating branch ${branchName} with committish origin/{branchName}");
                var branch = this.Repository.CreateBranch(this._options.ProjectVersion, $"origin/{this._options.ProjectVersion}");
                this._logger.LogDebug($"Checking out branch ${branchName}");
                Commands.Checkout(this.Repository, branch);

                this._logger.LogDebug($"Creating branch ${this._options.ProjectVersion} with committish origin/{this._options.ProjectVersion}");
                var currentBranch = this.Repository.CreateBranch(this._options.ProjectVersion, $"origin/{this._options.ProjectVersion}");
                this._logger.LogDebug($"Checking out branch ${this._options.ProjectVersion}");
                Commands.Checkout(this.Repository, currentBranch);
            }
            catch (Exception e)
            {
                this._logger.LogDebug($"Something went wrong during checkout.\n{e.Message}");
                // Do nothing, Checkout is already done
            }
        }

        private string GetFriendlyName(string canonicalBranchName)
        {
            return string.Join('/', canonicalBranchName.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(2));
        }

        public Commit DetermineCommit()
        {
            var (gitSourceKind, commit) = this.GetCommit();

            if (commit == null)
            {
                this.Checkout(gitSourceKind);
                (_, commit) = this.GetCommit();
            }

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given source {this._options.GitSource}. Please provide a different --git-source or remove this option.");
            }

            return commit;
        }


        private (GitSourceKinds, Commit) GetCommit()
        {
            GitSourceKinds gitSourceKind = GitSourceKinds.Unknown;
            Branch sourceBranch = null;
            foreach (var branch in this.Repository.Branches)
            {
                try
                {
                    if (branch.CanonicalName == this._options.GitSource)
                    {
                        sourceBranch = branch;
                        gitSourceKind = GitSourceKinds.CanonicalBranchName;
                        break;
                    }
                    if (branch.FriendlyName == this._options.GitSource)
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

            if (this._options.GitSource.Length == 40)
            {
                var commit = this.Repository.Lookup(new ObjectId(this._options.GitSource)) as Commit;

                if (commit != null)
                {
                    this._logger.LogDebug($"Found commit {commit.Sha} for commit {this._options.GitSource}");
                    return (GitSourceKinds.Commit, commit);
                }
            }

            return (gitSourceKind, null);
        }
    }
}
