using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.Core.DashboardCompare
{
    public class GitBranchProvider : IBranchProvider
    {
        private readonly StrykerOptions _options;
        private readonly IRepository _repository;
        private const string NoBranch = "(no branch)";
        public GitBranchProvider(StrykerOptions options, IRepository repository = null)
        {
            _options = options;

            if (repository != null)
            {
                _repository = repository;
            }
            else
            {
                _repository = CreateRepository();
            }

        }

        public string GetCurrentBranchName()
        {
            if (_repository?.Branches != null)
            {
                foreach (var branch in _repository.Branches)
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

            string repositoryPath = Repository.Discover(_options.BasePath)?.Split(".git")[0];

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }
            else
            {
                return new Repository(repositoryPath);
            }
        }

        public void Checkout()
        {
            var currentBranch = _repository.Branches[_options.ProjectVersion];
            if (currentBranch != null)
            {
                Commands.Checkout(_repository, currentBranch);
            }

        }
    }
}
