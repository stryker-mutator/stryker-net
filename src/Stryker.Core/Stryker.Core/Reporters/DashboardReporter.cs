using Microsoft.Extensions.Logging;
using Spectre.Console;
using Stryker.Core.Clients;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;

namespace Stryker.Core.Reporters
{
    public class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IDashboardClient _dashboardClient;
        private readonly ILogger<DashboardReporter> _logger;
        private readonly IAnsiConsole _console;
        private readonly IWebbrowserOpener _processWrapper;

        public DashboardReporter(StrykerOptions options, IDashboardClient dashboardClient = null, ILogger<DashboardReporter> logger = null,
            IAnsiConsole console = null, IWebbrowserOpener processWrapper = null)
        {
            _options = options;
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
            _console = console ?? AnsiConsole.Console;
            _processWrapper = processWrapper ?? new WebbrowserOpener();
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);

            var reportUri = _dashboardClient.PublishReport(mutationReport, _options.ProjectVersion).Result;

            if (reportUri != null)
            {
                if (_options.ReportTypeToOpen == Options.Inputs.ReportType.Dashboard)
                {
                    _processWrapper.Open(reportUri);
                }
                else
                {
                    _console.Write("[Cyan]Hint: by passing \"--open-report:dashboard or -o:dashboard\" the report will open automatically once Stryker is done.[/]");
                }

                _logger.LogDebug("Your stryker report has been uploaded to: \n {0} \nYou can open it in your browser of choice.", reportUri);
                _console.Write($"[Green]Your stryker report has been uploaded to: \n {reportUri} \nYou can open it in your browser of choice.[/]");
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }

            _console.WriteLine();
            _console.WriteLine();
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
            // Method to implement the interface
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // Method to implement the interface
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
            // Method to implement the interface
        }
    }
}
