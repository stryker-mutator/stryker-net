using LibGit2Sharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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

        private async Task<JsonReport> GetBaseline()
        {
            var branchName = _branchProvider.GetCurrentBranchCanonicalName();

            _options.CurrentBranchCanonicalName = branchName;

            var report = await _dashboardClient.PullReport(branchName);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project {0}, now trying fallback Version", _options.ProjectName);

                return await GetFallbackBaseline();
            }

            _logger.LogInformation("Found report of project {0} using version {1} ", _options.ProjectName, branchName);

            return report;
        }

        public async Task<JsonReport> GetFallbackBaseline()
        {
            var report = await _dashboardClient.PullReport(_options.FallbackVersion);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project using fallback version. Now running a complete test to establish a baseline.");
                return null;
            }
            else
            {
                _logger.LogInformation("Found report of project {0} using version {1}", _options.ProjectName, _options.FallbackVersion);

                return report;
            }
        }


        public async Task UpdateFolderCompositeCacheWithBaseline()
        {
            var baseline = await GetBaseline();

            var cache = FolderCompositeCache.Instance.Cache;

            foreach (var baselineFile in baseline.Files)
            {
                var filePath = FilePathUtils.NormalizePathSeparators(baselineFile.Key);
                var fileName = Path.GetFileName(filePath);
                var directoryName = Path.GetDirectoryName(filePath);

                var cacheFile = cache[directoryName].Children.FirstOrDefault(x => x.Name == fileName);

                foreach (var baselineMutant in baselineFile.Value.Mutants)
                {
                    var baselineSourceCode = baselineFile.Value.Source;

                    var mutantSource = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);
                }

            }
        }


        public string GetMutantSourceCode(string source, JsonMutant mutant)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

            var beginLinePosition = new LinePosition(mutant.Location.Start.Line - 1, mutant.Location.Start.Column - 1);
            var endLinePosition = new LinePosition(mutant.Location.End.Line - 1, mutant.Location.End.Column - 1);

            LinePositionSpan span = new LinePositionSpan(beginLinePosition, endLinePosition);

            var textSpan = tree.GetText().Lines.GetTextSpan(span);

            return tree.GetRoot().DescendantNodes(textSpan)
                .First(n => textSpan.Contains(n.Span)).ToString();
            
        }

        public async Task<DiffResult> ScanDiff()
        {
            if (_options.DiffCompareToDashboard)
            {
                await UpdateFolderCompositeCacheWithBaseline();
            }

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
