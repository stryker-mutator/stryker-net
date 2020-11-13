using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.MutantFilters
{
    public class DashboardMutantFilter : IMutantFilter
    {

        private readonly IBaselineProvider _baselineProvider;
        private readonly IGitInfoProvider _gitInfoProvider;
        private readonly ILogger<DashboardMutantFilter> _logger;

        private readonly StrykerOptions _options;
        private readonly JsonReport _baseline;

        public string DisplayName => "dashboard filter";

        public DashboardMutantFilter(StrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardMutantFilter>();
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
            _options = options;

            if (options.CompareToDashboard)
            {
                _baseline = GetBaselineAsync().Result;
            }
        }
        

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, StrykerOptions options)
        {
            if (options.CompareToDashboard)
            {
                if (_baseline == null)
                {
                    _logger.LogDebug("Returning all mutants on {0} because there is no baseline available", file.RelativePathToProjectFile);
                }
                else
                {
                    UpdateMutantsWithBaselineStatus(mutants, file);
                }
            }

            return mutants;
        }

        private void UpdateMutantsWithBaselineStatus(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file)
        {
            var baselineFile = _baseline.Files.SingleOrDefault(f => FilePathUtils.NormalizePathSeparators(f.Key) == FilePathUtils.NormalizePathSeparators(file.RelativePath));

            if (baselineFile is { } && baselineFile.Value is { })
            {
                foreach (var baselineMutant in baselineFile.Value.Mutants)
                {
                    var baselineMutantSourceCode = GetMutantSourceCode(baselineFile.Value.Source, baselineMutant);

                    if (string.IsNullOrEmpty(baselineMutantSourceCode))
                    {
                        _logger.LogWarning("Unable to find mutant span in original baseline source code. This indicates a bug in stryker. Please report this on github.");
                        continue;
                    }

                    IEnumerable<Mutant> matchingMutants = GetMutantMatchingSourceCode(mutants, baselineMutant, baselineMutantSourceCode);

                    SetMutantStatusToBaselineMutantStatus(baselineMutant, matchingMutants);
                }
            }
        }

        private void SetMutantStatusToBaselineMutantStatus(JsonMutant baselineMutant, IEnumerable<Mutant> matchingMutants)
        {
            if (matchingMutants.Count() == 1)
            {
                matchingMutants.First().ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
                matchingMutants.First().ResultStatusReason = "Result based on previous run";
            }
            else
            {
                foreach (var matchingMutant in matchingMutants)
                {
                    matchingMutant.ResultStatus = MutantStatus.NotRun;
                    matchingMutant.ResultStatusReason = "Result based on previous run was inconclusive";
                }
            }
        }

        private IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode)
        {
            return mutants.Where(x =>
               x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode &&
               x.Mutation.DisplayName == baselineMutant.MutatorName);
        }

        public string GetMutantSourceCode(string source, JsonMutant baselineMutant)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

            var beginLinePosition = new LinePosition(baselineMutant.Location.Start.Line - 1, baselineMutant.Location.Start.Column - 1);
            var endLinePosition = new LinePosition(baselineMutant.Location.End.Line - 1, baselineMutant.Location.End.Column - 1);

            var span = new LinePositionSpan(beginLinePosition, endLinePosition);

            var textSpan = tree.GetText().Lines.GetTextSpan(span);
            var originalNode = tree.GetRoot().DescendantNodes(textSpan).FirstOrDefault(n => textSpan.Equals(n.Span));
            return originalNode?.ToString();
        }

        private async Task<JsonReport> GetBaselineAsync()
        {
            var branchName = _gitInfoProvider.GetCurrentBranchName();

            var baselineLocation = $"dashboard-compare/{branchName}";

            var report = await _baselineProvider.Load(baselineLocation);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline for branch {0}, now trying fallback version {1}", branchName, _options.FallbackVersion);

                return await GetFallbackBaselineAsync();
            }

            _logger.LogInformation("Found baseline report for current branch {0}", branchName);

            return report;
        }

        private async Task<JsonReport> GetFallbackBaselineAsync()
        {
            var report = await _baselineProvider.Load(_options.FallbackVersion);

            if (report == null)
            {
                _logger.LogInformation("We could not locate a baseline report for the current branch or fallback version. Now running a complete test to establish a baseline.");
                return null;
            }

            _logger.LogInformation("Found fallback report using version {0}", _options.FallbackVersion);

            return report;
        }
    }
}
