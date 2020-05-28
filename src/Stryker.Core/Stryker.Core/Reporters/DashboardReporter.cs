using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stryker.Core.Reporters
{
    public partial class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IDashboardClient _dashboardClient;
        private readonly IBranchProvider _branchProvider;
        private readonly ILogger<DashboardReporter> _logger;

        public DashboardReporter(StrykerOptions options, IDashboardClient dashboardClient = null, IBranchProvider branchProvider = null)
        {
            _options = options;
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _branchProvider = branchProvider ?? new GitBranchProvider(options);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);


            if (_options.CompareToDashboard)
            {
                Task.WaitAll(UploadHumanReadableReport(mutationReport), UploadBaseline(mutationReport));
            }
            else
            {
                Task.WaitAll(UploadHumanReadableReport(mutationReport));
            }


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
            var branchName = _branchProvider.GetCurrentBranchName();

            var projectVersion = !string.IsNullOrEmpty(branchName) ? branchName : _options.FallbackVersion;

            var reportUrl = await _dashboardClient.PublishReport(mutationReport.ToJson(), projectVersion);

            if (reportUrl != null)
            {
                _logger.LogDebug($"\nYour baseline stryker report has been uploaded to: \n {reportUrl} \nYou can open it in your browser of choice.");
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }
        }

        private async Task UploadHumanReadableReport(JsonReport mutationReport)
        {
            var reportUrl = await _dashboardClient.PublishReport(mutationReport.ToJson(), _options.ProjectVersion);

            if (reportUrl != null)
            {
                _logger.LogInformation("\nYour stryker report has been uploaded to: \n {0} \nYou can open it in your browser of choice.", reportUrl);
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}
