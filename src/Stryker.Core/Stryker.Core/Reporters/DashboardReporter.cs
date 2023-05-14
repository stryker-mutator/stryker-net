using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Stryker.Core.Clients;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.WebBrowserOpener;

namespace Stryker.Core.Reporters
{
    public class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IDashboardClient _dashboardClient;
        private readonly ILogger<DashboardReporter> _logger;
        private readonly IAnsiConsole _console;
        private readonly IWebbrowserOpener _browser;

        public DashboardReporter(StrykerOptions options, IDashboardClient dashboardClient = null, ILogger<DashboardReporter> logger = null,
            IAnsiConsole console = null, IWebbrowserOpener browser = null)
        {
            _options = options;
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
            _console = console ?? AnsiConsole.Console;
            _browser = browser ?? new CrossPlatformBrowserOpener();
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);

            var reportUri = _dashboardClient.PublishReport(mutationReport, _options.ProjectVersion).Result;

            if (reportUri != null)
            {
                if (_options.ReportTypeToOpen == Options.Inputs.ReportType.Dashboard)
                {
                    _browser.Open(reportUri);
                }
                else
                {
                    var aqua = new Style(Color.Aqua);
                    _console.WriteLine("Hint: by passing \"--open-report:dashboard or -o:dashboard\" the report will open automatically once Stryker is done.", aqua);
                }

                var green = new Style(Color.Green);
                _console.WriteLine();
                _console.WriteLine("Your report has been uploaded at:", green);
                // We must print the report path as the link text because on some terminals links might be supported but not actually clickable: https://github.com/spectreconsole/spectre.console/issues/764
                _console.WriteLine(reportUri, _console.Profile.Capabilities.Links ? green.Combine(new Style(link: reportUri)) : green);
                _console.WriteLine("You can open it in your browser of choice.", green);
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }

            _console.WriteLine();
            _console.WriteLine();
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
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
