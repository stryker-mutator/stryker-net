using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Clients;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Reporters
{
    public partial class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IChalk _chalk;
        private readonly IDashboardClient _dashboardClient;

        public DashboardReporter(StrykerOptions options, IChalk chalk = null, IDashboardClient dashboardClient = null)
        {
            _options = options;
            _chalk = chalk ?? new Chalk();
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);

            var projectVersion = _options.ProjectVersion;

            if (_options.DiffCompareToDashboard && _options.CurrentBranchCanonicalName != string.Empty)
            {
                projectVersion = _options.CurrentBranchCanonicalName;
            }

            var reportUrl = _dashboardClient.PublishReport(mutationReport.ToJson(), projectVersion).Result;

            if (reportUrl != null)
            {
                _chalk.Green($"\nYour stryker report has been uploaded to: \n {reportUrl} \nYou can open it in your browser of choice.");
            }
            else
            {
                _chalk.Red("Uploading to stryker dashboard failed...");
            }
            Console.WriteLine(Environment.NewLine);
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
            // Method to implement the interface
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // Method to implement the interface
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            // Method to implement the interface
        }
    }
}
