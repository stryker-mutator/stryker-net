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
        private readonly ILogger<DashboardReporter> _logger;

        public DashboardReporter(StrykerOptions options, IChalk chalk = null, IDashboardClient dashboardClient = null)
        {
            _options = options;
            _chalk = chalk ?? new Chalk();
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);

            Task.WaitAll(UploadBaseline(mutationReport), UploadHumanReadableReport(mutationReport));
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

        private async Task UploadBaseline(JsonReport mutationReport)
        {

            var projectVersion = _options.FallbackVersion;

            if (_options.DiffCompareToDashboard && _options.CurrentBranchCanonicalName != string.Empty)
            {
                projectVersion = _options.CurrentBranchCanonicalName;
            }
            var reportUrl = await _dashboardClient.PublishReport(mutationReport.ToJson(), projectVersion);

            if (reportUrl != null)
            {
                _logger.LogDebug($"\nYour baseline stryker report has been uploaded to: \n {reportUrl} \nYou can open it in your browser of choice.");
            }
            else
            {
                _logger.LogDebug("Uploading to stryker dashboard failed...");
            }
        }

        private async Task UploadHumanReadableReport(JsonReport mutationReport)
        {
            var reportUrl = await _dashboardClient.PublishReport(mutationReport.ToJson(), _options.ProjectVersion);

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
    }
}
