using Stryker.Core.Baseline;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Reporters
{
    public class GitBaselineReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IBaselineProvider _baselineProvider; 
        public GitBaselineReporter(StrykerOptions options, IBaselineProvider baselineProvider = null)
        {
            _options = options;
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
        }
        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);
            var projectVersion = _options.CurrentBranchCanonicalName ?? _options.FallbackVersion;

            _baselineProvider.Save(mutationReport, projectVersion);
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
