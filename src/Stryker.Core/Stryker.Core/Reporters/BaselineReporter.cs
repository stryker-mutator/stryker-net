using System.Collections.Generic;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters
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
