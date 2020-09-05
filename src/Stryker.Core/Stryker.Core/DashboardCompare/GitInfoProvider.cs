using LibGit2Sharp;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using System;

namespace Stryker.Core.DashboardCompare
{
    public class GitInfoProvider : IGitInfoProvider
    {
        private readonly StrykerOptions _options;
        private readonly string _repositoryPath;
        private readonly ILogger<GitInfoProvider> _logger;

        public IRepository Repository { get; }

        public string RepositoryPath
        {
            get
            {
                return _repositoryPath ?? LibGit2Sharp.Repository.Discover(_options.BasePath)?.Split(".git")[0];
            }
        }

        public GitInfoProvider(StrykerOptions options, IRepository repository = null, string repositoryPath = null, ILogger<GitInfoProvider> logger = null)
        { 
            _repositoryPath = repositoryPath;
            _options = options;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<GitInfoProvider>();

            if (!options.DiffEnabled)
            {
                return;
            }

            if (repository != null)
            {
                Repository = repository;
            }
            else
            {
                Repository = CreateRepository();
            }
        }

        public string GetCurrentBranchName()
        {
            if (Repository?.Branches == null)
            {
                Checkout();
            }

            if (Repository?.Branches != null)
            {
                foreach (var branch in Repository.Branches)
                {
                    if (branch.IsCurrentRepositoryHead)
                    {
                        return branch.FriendlyName;
                    }
                }
            }

            return string.Empty;
        }

        private IRepository CreateRepository()
        {
            if (string.IsNullOrEmpty(RepositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }
            else
            {
                return new Repository(RepositoryPath);
            }
        }

        private void Checkout()
        {
            try
            {
                var branch = Repository.CreateBranch(_options.GitSource, $"origin/{_options.GitSource}");

                Commands.Checkout(Repository, branch);

                var currentBranch = Repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");

                Commands.Checkout(Repository, currentBranch);
            }
            catch(Exception e)
            {
                _logger.LogWarning(e.Message);
            }
        }

        public Commit DetermineCommit()
        {
            var commit = GetCommit();

            if (commit == null)
            {
                Checkout();
                commit = GetCommit();
            }

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given source {_options.GitSource}. Please provide a different --git-source or remove this option.");
            }

            return commit;
        }


        private Commit GetCommit()
        {
            Branch sourceBranch = null;
            foreach (var branch in Repository.Branches)
            {
                try
                {
                    if (branch.CanonicalName == _options.GitSource || branch.FriendlyName == _options.GitSource)
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

            if (_options.GitSource.Length == 40)
            {
                var commit = Repository.Lookup(new ObjectId(_options.GitSource)) as Commit;

                if (commit != null)
                {
                    return commit;
                }
            }

            return null;
        }
    }
}
