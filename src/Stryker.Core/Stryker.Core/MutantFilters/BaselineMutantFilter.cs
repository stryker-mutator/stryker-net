using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
using Stryker.Utilities;

namespace Stryker.Core.MutantFilters;

public class BaselineMutantFilter : IMutantFilter
{
    private readonly IBaselineProvider _baselineProvider;
    private readonly IGitInfoProvider _gitInfoProvider;
    private readonly ILogger<BaselineMutantFilter> _logger;
    private readonly IBaselineMutantHelper _baselineMutantHelper;

    private readonly IStrykerOptions _options;
    private readonly IJsonReport _baseline;

    public MutantFilter Type => MutantFilter.Baseline;
    public string DisplayName => "baseline filter";

    public BaselineMutantFilter(IStrykerOptions options, IBaselineProvider baselineProvider = null,
        IGitInfoProvider gitInfoProvider = null, IBaselineMutantHelper baselineMutantHelper = null)
    {
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<BaselineMutantFilter>();
        _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
        _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        _baselineMutantHelper = baselineMutantHelper ?? new BaselineMutantHelper();

        _options = options;

        if (options.WithBaseline)
        {
            _baseline = GetBaselineAsync().Result;
        }
    }


    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file,
        IStrykerOptions options)
    {
        if (options.WithBaseline)
        {
            if (_baseline == null)
            {
                _logger.LogDebug(
                    "Returning all mutants on {RelativeFilePath} because there is no baseline available",
                    file.RelativePath);
            }
            else
            {
                UpdateMutantsWithBaselineStatus(mutants, file);
            }
        }

        return mutants;
    }

    private void UpdateMutantsWithBaselineStatus(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file)
    {
        if (!_baseline.Files.ContainsKey(FilePathUtils.NormalizePathSeparators(file.RelativePath)))
        {
            return;
        }

        var baselineFile = _baseline.Files[FilePathUtils.NormalizePathSeparators(file.RelativePath)];

        if (baselineFile is { })
        {
            foreach (var baselineMutant in baselineFile.Mutants)
            {
                var baselineMutantSourceCode =
                    _baselineMutantHelper.GetMutantSourceCode(baselineFile.Source, baselineMutant);

                if (string.IsNullOrEmpty(baselineMutantSourceCode))
                {
                    _logger.LogWarning(
                        "Unable to find mutant span in original baseline source code. This indicates a bug in stryker. Please report this on github.");
                    continue;
                }

                var matchingMutants =
                    _baselineMutantHelper.GetMutantMatchingSourceCode(mutants, baselineMutant,
                        baselineMutantSourceCode);

                SetMutantStatusToBaselineMutantStatus(baselineMutant, matchingMutants);
            }
        }
    }

    private static void SetMutantStatusToBaselineMutantStatus(IJsonMutant baselineMutant,
        IEnumerable<IMutant> matchingMutants)
    {
        if (matchingMutants.Count() == 1)
        {
            var matchingMutant = matchingMutants.First();
            matchingMutant.ResultStatus = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
            matchingMutant.ResultStatusReason = "Result based on previous run";
        }
        else
        {
            foreach (var matchingMutant in matchingMutants)
            {
                matchingMutant.ResultStatus = MutantStatus.Pending;
                matchingMutant.ResultStatusReason = "Result based on previous run was inconclusive";
            }
        }
    }

    private async Task<IJsonReport> GetBaselineAsync()
    {
        var branchName = _gitInfoProvider.GetCurrentBranchName();

        var baselineLocation = $"baseline/{branchName}";

        var report = await _baselineProvider.Load(baselineLocation);

        if (report == null)
        {
            _logger.LogInformation(
                "We could not locate a baseline for branch {BranchName}, now trying fallback version {FallbackVersion}",
                branchName, _options.FallbackVersion);

            return await GetFallbackBaselineAsync();
        }

        _logger.LogInformation("Found baseline report for current branch {BranchName}", branchName);

        return report;
    }

    private async Task<IJsonReport> GetFallbackBaselineAsync(bool baseline = true)
    {
        var report = await _baselineProvider.Load($"{(baseline ? "baseline/" : "")}{_options.FallbackVersion}");

        if (report == null)
        {
            if (baseline)
            {
                _logger.LogDebug(
                    "We could not locate a baseline report for the fallback version. Now trying regular fallback version.");
                return await GetFallbackBaselineAsync(false);
            }

            _logger.LogInformation(
                "We could not locate a baseline report for the current branch, version or fallback version. Now running a complete test to establish a fresh baseline.");
            return null;
        }

        _logger.LogInformation("Found fallback report using version {FallbackVersion}", _options.FallbackVersion);

        return report;
    }
}
