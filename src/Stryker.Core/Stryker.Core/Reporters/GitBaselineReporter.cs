using Stryker.Core.Baseline;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;

namespace Stryker.Core.Reporters
{
    public class GitBaselineReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IBaselineProvider _baselineProvider;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitBaselineReporter(StrykerOptions options, IBaselineProvider baselineProvider = null, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);
            var projectVersion = _gitInfoProvider.GetCurrentBranchName() ?? _options.ProjectVersion;
            var baselineVersion = $"dashboard-compare/{projectVersion}";

            _baselineProvider.Save(mutationReport, baselineVersion).Wait();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
            // For implementing interface
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // For implementing interface
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            // For implementing interface
        }
    }
}
