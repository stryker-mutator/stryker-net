using Crayon;
using Microsoft.Extensions.Logging;
using Stryker.Core.Clients;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stryker.Core.Reporters
{
    public class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IDashboardClient _dashboardClient;
        private readonly ILogger<DashboardReporter> _logger;
        private readonly TextWriter _consoleWriter;
        private readonly IProcessWrapper _processWrapper;

        public DashboardReporter(StrykerOptions options, IDashboardClient dashboardClient = null, ILogger<DashboardReporter> logger = null,
            TextWriter consoleWriter = null, IProcessWrapper processWrapper = null)
        {
            _options = options;
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();
            _consoleWriter = consoleWriter ?? Console.Out;
            _processWrapper = processWrapper ?? new ProcessWrapper();
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);

            var reportUri = _dashboardClient.PublishReport(mutationReport, _options.ProjectVersion).Result;

            if (reportUri != null)
            {
                _logger.LogDebug("Your stryker report has been uploaded to: \n {0} \nYou can open it in your browser of choice.", reportUri);
                _consoleWriter.Write(Output.Green($"Your stryker report has been uploaded to: \n {reportUri} \nYou can open it in your browser of choice."));

                if (_options.ReportTypeToOpen == Options.Inputs.ReportType.Dashboard)
                {
                    _processWrapper.StartUrl(reportUri);
                }
            }
            else
            {
                _logger.LogError("Uploading to stryker dashboard failed...");
            }

            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine();
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
