using System.Collections.Generic;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Baseline.Providers;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.ProjectComponents.TestProjects;
using Stryker.Abstractions.Reporters.Json;
using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.Reporters
{
    public class BaselineReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IBaselineProvider _baselineProvider;
        private readonly IGitInfoProvider _gitInfoProvider;

        public BaselineReporter(StrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);
            var projectVersion = _gitInfoProvider.GetCurrentBranchName();
            var baselineVersion = $"baseline/{projectVersion}";

            _baselineProvider.Save(mutationReport, baselineVersion).Wait();
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
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
}
