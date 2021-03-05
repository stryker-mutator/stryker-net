using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
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
        private readonly IBaselineMutantHelper _baselineMutantHelper;

        private readonly IStrykerOptions _options;
        private readonly JsonReport _baseline;

        public string DisplayName => "dashboard filter";

        public DashboardMutantFilter(IStrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null, IBaselineMutantHelper baselineMutantHelper = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardMutantFilter>();
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
            _baselineMutantHelper = baselineMutantHelper ?? new BaselineMutantHelper();

            _options = options;

            if (options.CompareToDashboard)
            {
                _baseline = GetBaselineAsync().Result;
            }
        }
        

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            if (options.CompareToDashboard)
            {
                if (_baseline == null)
                {
                    _logger.LogDebug("Returning all mutants on {0} because there is no baseline available", file.RelativePath);
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
            JsonReportFileComponent baselineFile = _baseline.Files[FilePathUtils.NormalizePathSeparators(file.RelativePath)];

            if (baselineFile is { })
            {
                foreach (var baselineMutant in baselineFile.Mutants)
                {
                    var baselineMutantSourceCode = _baselineMutantHelper.GetMutantSourceCode(baselineFile.Source, baselineMutant);

                    if (string.IsNullOrEmpty(baselineMutantSourceCode))
                    {
                        _logger.LogWarning("Unable to find mutant span in original baseline source code. This indicates a bug in stryker. Please report this on github.");
                        continue;
                    }

                    IEnumerable<Mutant> matchingMutants = _baselineMutantHelper.GetMutantMatchingSourceCode(mutants, baselineMutant, baselineMutantSourceCode);

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
