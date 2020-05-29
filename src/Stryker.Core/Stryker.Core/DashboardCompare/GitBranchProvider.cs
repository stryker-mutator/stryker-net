using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.Core.DashboardCompare
{
    public class GitBranchProvider : IBranchProvider
    {
        private readonly StrykerOptions _options;
        private readonly IRepository _repository;
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
            Checkout();
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
            try
            {
                var branch = _repository.CreateBranch(_options.GitSource, $"origin/{_options.GitSource}");

                Commands.Checkout(_repository, branch);

                var currentBranch = _repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");

                Commands.Checkout(_repository, currentBranch);
            }
            catch
            {
                // Do nothing, Checkout is already done
            }
        }
    }
}
