using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.BranchProvider;
using Stryker.Core.Clients;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        private readonly ILogger<GitDiffProvider> _logger;
        private readonly StrykerOptions _options;
        private readonly IBranchProvider _branchProvider;
        private readonly IDashboardClient _dashboardClient;

        public GitDiffProvider(StrykerOptions options, IBranchProvider branchProvider = null, IDashboardClient dashboardClient = null)
        {
            _logger = Logging.ApplicationLogging.LoggerFactory.CreateLogger<GitDiffProvider>();
            _options = options;
            _branchProvider = branchProvider ?? new GitBranchProvider(options);
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
        }

        private async Task BaselineExists()
        {
            var branchName = _branchProvider.GetCurrentBranchCanonicalName();

            _options.CurrentBranchCanonicalName = branchName;

            var report = await _dashboardClient.PullReport(branchName);

            _options.Baseline = report;

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project {0}, running a full test to create initial baseline", _options.ProjectName);
            } else
            {
                _logger.LogInformation("Found report of project {0}", _options.ProjectName);
            }
        }


        public async Task<DiffResult> ScanDiff()
        {

            await BaselineExists();

            string repositoryPath = Repository.Discover(_options.BasePath)?.Split(".git")[0];

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }
            var diffResult = new DiffResult()
            {
                ChangedFiles = new Collection<string>(),
                TestsChanged = false
            };

            // A git repository has been detected, calculate the diff to filter
            using (var repository = new Repository(repositoryPath))
            {
                var sourceBranch = repository.Branches[_options.GitSource];

                if (sourceBranch == null)
                {
                    throw new StrykerInputException("Git source branch does not exist. Please set another source branch or remove the --git-source option.");
                }

                else
                {
                    foreach (var treeChanges in repository.Diff.Compare<TreeChanges>(sourceBranch.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                    {
                        string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(repositoryPath, treeChanges.Path));
                        diffResult.ChangedFiles.Add(diffPath);
                        if (diffPath.StartsWith(_options.BasePath))
                        {
                            diffResult.TestsChanged = true;
                        }
                    }
                }

                
            }
            return diffResult;
        }
    }
}
