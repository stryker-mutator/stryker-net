using System.Collections.Generic;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters;

public class BaselineReporter : IReporter
{
    private readonly IStrykerOptions _options;
    private readonly IBaselineProvider _baselineProvider;
    private readonly IGitInfoProvider _gitInfoProvider;

    public BaselineReporter(IStrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
    {
        _options = options;
        _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
        _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);
        var projectVersion = _gitInfoProvider.GetCurrentBranchName();
        var baselineVersion = $"baseline/{projectVersion}";

        _baselineProvider.Save(mutationReport, baselineVersion).Wait();
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        // This reporter does not report during the testrun
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        // This reporter does not report during the testrun
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
        // This reporter does not report during the testrun
    }
}
