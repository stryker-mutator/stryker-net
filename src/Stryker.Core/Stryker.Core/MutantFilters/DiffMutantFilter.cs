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
                if (_baseline == null)
                {
                    return mutants;
                }

                UpdateMutantsWithBaseline(mutants, file);
            }

            if (!_diffResult.TestsChanged)
            {
                if (_diffResult.ChangedFiles.Contains(file.FullPath))
                {
                    return SetMutantStatusForFileChanged(mutants);
                }

                if (_options.CompareToDashboard)
                {
                    return FilterMutantsWithStatusUnclear(mutants);
                }

                return Enumerable.Empty<Mutant>();
            }

            return ResetMutantStatus(mutants);

        }

        private IEnumerable<Mutant> FilterMutantsWithStatusUnclear(IEnumerable<Mutant> mutants)
        {
            var uncheckedMutants = new List<Mutant>();
            foreach (var mutant in mutants)
            {
                if (mutant.ResultStatusReason == "Could not determine the correct mutant status")
                {
                    uncheckedMutants.Add(mutant);
                }
            }

            return uncheckedMutants;
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
            var branchName = _branchProvider.GetCurrentBranchCanonicalName();

            var report = await _dashboardClient.PullReport(branchName);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for project {0}, now trying fallback Version", _options.ProjectName);

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

                        IEnumerable<Mutant> matchingMutants = GetMatchingMutants(mutants, baselineMutant, baselineMutantSourceCode);

                        if (matchingMutants.Count() == 1)
                        {
                            UpdateMutantStatusWithBaseline(baselineMutant, matchingMutants);
                        }
                        else
                        {
                            UpdateMutantStatusForNoCertainResult(matchingMutants);
                        }
                    }
                }
            }
        }

        private void UpdateMutantStatusWithBaseline(JsonMutant baselineMutant, IEnumerable<Mutant> matchingMutants)
        {
            matchingMutants.First().ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
            matchingMutants.First().ResultStatusReason = "Result based on previous run.";
        }

        private IEnumerable<Mutant> GetMatchingMutants(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode)
        {
            return mutants.Where(x =>
                x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode &&
                x.Mutation.DisplayName == baselineMutant.MutatorName);
        }

        private void UpdateMutantStatusForNoCertainResult(IEnumerable<Mutant> matchingMutants)
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
