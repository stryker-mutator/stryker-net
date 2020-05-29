using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
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

        private readonly JsonReport _baseline;

        private readonly ILogger<DiffMutantFilter> _logger;

        public string DisplayName => "git diff file filter";

        public DiffMutantFilter(StrykerOptions options = null, IDiffProvider diffProvider = null, IDashboardClient dashboardClient = null, IBranchProvider branchProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();

            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _branchProvider = branchProvider ?? new GitBranchProvider(options);
            _options = options;

            if (options.CompareToDashboard)
            {
                _baseline = GetBaseline().Result;
            }

            _diffResult = diffProvider.ScanDiff();
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (options.CompareToDashboard)
            {
                // If the dashboard feature is enabled but we cannot find a baseline. We are going to test the entire project. Thus none of the mutants can be filtered out and all are returned.
                if (_baseline == null)
                {
                    _logger.LogDebug("Testing all mutants on {0} because there is no baseline", file.RelativePathToProjectFile);
                    return mutants;
                }

                // Updates all the mutants in this file with their counterpart's result in the report of the previous run.
                UpdateMutantsWithBaseline(mutants, file);
            }

            // We check if the tests have changed, if this is the case we should run all mutants. Otherwise we start filtering.
            if (!_diffResult.TestsChanged)
            {

                if (_diffResult.ChangedFiles.Contains(file.FullPath))
                {
                    // If the diffresult flags this file as modified. We want to run all mutants again.
                    return SetMutantStatusForFileChanged(mutants);
                }

                if (_options.CompareToDashboard)
                {
                    // When using the compare to dashboard feature. 
                    // Some mutants mutants have no certain result because we couldn't say with certainty which mutant on the dashboard belonged to it. 
                    //These mutants have to be reset and tested.
                    return ReturnMutantsWithStatusNotRun(mutants);
                }

                // If tests haven't changed and neither the file has changed or the compare feature is being used, we are not interested in the mutants of this file and thus can be filtered out completely.
                return Enumerable.Empty<Mutant>();
            }

            // If tests are changed, return all mutants with status set to NotRun. We cannot guarantee the result.
            return ResetMutantStatus(mutants);

        }

        private IEnumerable<Mutant> ReturnMutantsWithStatusNotRun(IEnumerable<Mutant> mutants)
        {
            var unclearMutants = new List<Mutant>();
            foreach (var mutant in mutants)
            {
                if (mutant.ResultStatus == MutantStatus.NotRun)
                {
                    unclearMutants.Add(mutant);
                }
            }

            return unclearMutants;
        }

        private IEnumerable<Mutant> SetMutantStatusForFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "File changed since last commit.";
            }
            return mutants;
        }

        private IEnumerable<Mutant> ResetMutantStatus(IEnumerable<Mutant> mutants)
        {
            // Set mutant status to not run because tests changed and all mutants must run again.
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
            }

            return mutants;
        }

        private async Task<JsonReport> GetBaseline()
        {
            var branchName = _branchProvider.GetCurrentBranchName();

            var baselineLocation = $"dashboard-compare/{branchName}";

            var report = await _dashboardClient.PullReport(baselineLocation);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project {0}, now trying fallback Version {1}", _options.ProjectName, _options.FallbackVersion);

                return await GetFallbackBaseline();
            }

            _logger.LogInformation("Found report of project {0} using version {1} ", _options.ProjectName, branchName);

            return report;
        }

        private async Task<JsonReport> GetFallbackBaseline()
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

        private void UpdateMutantsWithBaseline(IEnumerable<Mutant> mutants, FileLeaf file)
        {
            foreach (var baselineFile in _baseline.Files)
            {
                var filePath = FilePathUtils.NormalizePathSeparators(baselineFile.Key);

                if (filePath == file.RelativePath)
                {
                    foreach (var baselineMutant in baselineFile.Value.Mutants)
                    {
                        var baselineMutantSourceCode = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);

                        IEnumerable<Mutant> matchingMutants = GetMutantMatchingSourceCode(mutants, baselineMutant, baselineMutantSourceCode);

                        if (matchingMutants.Count() == 1)
                        {
                            UpdateMutantStatusWithBaseline(baselineMutant, matchingMutants.First());
                        }
                        else
                        {
                            UpdateMutantsForStatusUnclear(matchingMutants);
                        }
                    }
                }
            }

            var mutantGroups = mutants
                .GroupBy(x => x.ResultStatusReason)
                .OrderBy(x => x.Key);

            foreach (var skippedMutantGroup in mutantGroups)
            {
                _logger.LogInformation("{0} mutants got status {1}. Reason: {2}", skippedMutantGroup.Count(),
                    skippedMutantGroup.First().ResultStatus, skippedMutantGroup.Key);
            }
        }

        private void UpdateMutantStatusWithBaseline(JsonMutant baselineMutant, Mutant matchingMutants)
        {
            matchingMutants.ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
            matchingMutants.ResultStatusReason = "Result based on previous run.";
        }

        private IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode)
        {
            return mutants.Where(x =>
                x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode &&
                x.Mutation.DisplayName == baselineMutant.MutatorName);
        }

        private void UpdateMutantsForStatusUnclear(IEnumerable<Mutant> matchingMutants)
        {
            foreach (var matchingMutant in matchingMutants)
            {
                matchingMutant.ResultStatus = MutantStatus.NotRun;
                matchingMutant.ResultStatusReason = "Could not determine the correct mutant status";
            }
        }

        private string GetMutantSourceCode(string source, JsonMutant baselineMutant)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

            var beginLinePosition = new LinePosition(baselineMutant.Location.Start.Line - 1, baselineMutant.Location.Start.Column - 1);
            var endLinePosition = new LinePosition(baselineMutant.Location.End.Line - 1, baselineMutant.Location.End.Column - 1);

            LinePositionSpan span = new LinePositionSpan(beginLinePosition, endLinePosition);

            var textSpan = tree.GetText().Lines.GetTextSpan(span);

            return tree.GetRoot().DescendantNodes(textSpan)
                .First(n => textSpan.Equals(n.Span)).ToString();

        }
    }
}
