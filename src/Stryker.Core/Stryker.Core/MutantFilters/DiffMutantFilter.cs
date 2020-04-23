using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutantFilters
{
    public class DiffMutantFilter : IMutantFilter
    {
        private readonly DiffResult _diffResult;

        private readonly IDashboardClient _dashboardClient;
        private readonly IBranchProvider _branchProvider;

        private readonly StrykerOptions _options;
        private ILogger<DiffMutantFilter> _logger;
        private const string _displayName = "git diff file filter";
        public string DisplayName => _displayName;

        public DiffMutantFilter(StrykerOptions options, IDiffProvider diffProvider, IDashboardClient dashboardClient = null, IBranchProvider branchProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();

            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _branchProvider = branchProvider ?? new GitBranchProvider(options);
            _options = options;

            if (options.DiffEnabled)
            {
                _diffResult = diffProvider.ScanDiff();
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (options.DiffCompareToDashboard && BaselineReport.Instance.Report == null)
            {
                return mutants;
            }

            if (options.DiffEnabled && !_diffResult.TestsChanged)
            {
                if (_diffResult.ChangedFiles.Contains(file.FullPath))
                {
                    return mutants;
                }
                return Enumerable.Empty<Mutant>();
            }
            return mutants;
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

                    var mutantSource = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);

                    foreach (var cacheMutant in cacheFile.Mutants)
                    {
                        if (mutantSource.Equals(cacheMutant.Mutation.OriginalNode.ToString()))
                        {
                            cacheMutant.ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
                            cacheMutant.ResultStatusReason = "Based on previous run.";

                        }
                    }
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
                .First(n => textSpan.Equals(n.Span)).ToString();

        }
    }
}
