using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stryker.Core.Reporters
{
    public partial class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IDashboardClient _dashboardClient;
        private readonly IGitInfoProvider _gitInfoProvider;
        private readonly ILogger<DashboardReporter> _logger;
        private readonly IChalk _chalk;

        public DashboardReporter(StrykerOptions options, IDashboardClient dashboardClient = null, IGitInfoProvider gitInfoProvider = null, ILogger<DashboardReporter> logger = null, IChalk chalk = null)
        {
            _options = options;
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
            _chalk = chalk ?? new Chalk();
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
            var branchName = _gitInfoProvider.GetCurrentBranchName();

            var baselineLocation = $"dashboard-compare/{branchName}";


            var reportUrl = await _dashboardClient.PublishReport(mutationReport.ToJson(), baselineLocation);

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
                _logger.LogDebug("Your stryker report has been uploaded to: \n {0} \nYou can open it in your browser of choice.", reportUrl);
                _chalk.Green($"Your stryker report has been uploaded to: \n {reportUrl} \nYou can open it in your browser of choice.");
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}
