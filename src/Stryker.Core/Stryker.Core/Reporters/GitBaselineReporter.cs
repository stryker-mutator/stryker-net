using System.Collections.Generic;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters
{
    public class GitBaselineReporter : IReporter
    {
        private readonly IStrykerOptions _options;
        private readonly IBaselineProvider _baselineProvider;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitBaselineReporter(IStrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);
            var projectVersion = _gitInfoProvider.GetCurrentBranchName();
            var baselineVersion = $"dashboard-compare/{projectVersion}";

            _baselineProvider.Save(mutationReport, baselineVersion).Wait();
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
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
