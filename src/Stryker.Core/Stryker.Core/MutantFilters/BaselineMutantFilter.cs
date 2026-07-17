using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.DiffProviders;
using Stryker.Utilities;
using Stryker.Utilities.Logging;

namespace Stryker.Core.MutantFilters;

public class BaselineMutantFilter : IMutantFilter
{
    private readonly IBaselineProvider _baselineProvider;
    private readonly IGitInfoProvider _gitInfoProvider;
    private readonly ILogger<BaselineMutantFilter> _logger;
    private readonly IDiffProvider _diffProvider;
    private readonly IContentMutantMatcher _contentMatcher;

    private readonly IStrykerOptions _options;
    private readonly IJsonReport _baseline;

    public MutantFilter Type => MutantFilter.Baseline;
    public string DisplayName => "baseline filter";

    public BaselineMutantFilter(IStrykerOptions options, IBaselineProvider baselineProvider = null,
        IGitInfoProvider gitInfoProvider = null, IDiffProvider diffProvider = null, IContentMutantMatcher contentMatcher = null)
    {
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<BaselineMutantFilter>();
        _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
        _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        _diffProvider = diffProvider ?? new DiffMatchPatchProvider();
        _contentMatcher = contentMatcher ?? new ContentMutantMatcher();

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
            var contentDiff = _diffProvider.GetContentDiff(baselineFile.Source, file.SourceCode);

            foreach (var baselineMutant in baselineFile.Mutants)
            {
                var matchingMutants = _contentMatcher.MatchByLocation(mutants, baselineMutant, contentDiff);

                SetMutantStatusToBaselineMutantStatus(baselineMutant, matchingMutants);
            }
        }
    }

    private static void SetMutantStatusToBaselineMutantStatus(IJsonMutant baselineMutant,
        IEnumerable<IMutant> matchingMutants)
    {
        var matches = matchingMutants as ICollection<IMutant> ?? matchingMutants.ToList();
        if (matches.Count == 0)
        {
            return;
        }

        // Matching is now based on the mutant's remapped location rather than fragile source-text
        // equality, so multiple matches at the same location are no longer ambiguous: they all
        // correspond to the same baseline mutant and can safely reuse its result.
        var status = (MutantStatus)Enum.Parse(typeof(MutantStatus), baselineMutant.Status);
        foreach (var matchingMutant in matches)
        {
            matchingMutant.ResultStatus = status;
            matchingMutant.ResultStatusReason = "Result based on previous run";
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
