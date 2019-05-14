using Microsoft.Extensions.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(StrykerOptions options)
        {
            return new BroadcastReporter(DetermineEnabledReporters(options.Reporters, CreateReporters(options)));
        }

        private static IDictionary<Reporter, IReporter> CreateReporters(StrykerOptions options)
        {
            return new Dictionary<Reporter, IReporter>
            {
                { Reporter.Dots, new ConsoleDotProgressReporter() },
                { Reporter.Progress, CreateProgressReporter() },
                { Reporter.ClearText, new ConsoleReportReporter(options) },
                { Reporter.Json, new JsonReporter(options) },
                { Reporter.Html, new HtmlReporter(options) }
            };
        }

        private static IEnumerable<IReporter> DetermineEnabledReporters(IEnumerable<Reporter> enabledReporters, IDictionary<Reporter, IReporter> possibleReporters)
        {
            if (enabledReporters.Contains(Reporter.All))
            {
                return possibleReporters.Values;
            }
            var deprecated = new Dictionary<Reporter, Reporter>
            {
                { Reporter.ConsoleProgressBar, Reporter.Progress },
                { Reporter.ConsoleProgressDots, Reporter.Dots },
                { Reporter.ConsoleReport, Reporter.ClearText }
            };
            if(enabledReporters.Where(er => deprecated.Any(dr => dr.Key == er)) is var deprecatedReporters && deprecatedReporters.Count() > 0)
            {
                var logger = Logging.ApplicationLogging.LoggerFactory.CreateLogger(typeof(ReporterFactory).Name);
                foreach (var deprecatedReporter in deprecatedReporters)
                {
                    logger.LogWarning($"Reporter {deprecatedReporter.ToString()} is deprecated. Please use {deprecated[deprecatedReporter].ToString()} instead.");
                }
            }

            return possibleReporters.Where(reporter => enabledReporters.Contains(reporter.Key))
                .Select(reporter => reporter.Value);
        }

        private static ProgressReporter CreateProgressReporter()
        {
            var consoleOneLineLoggerFactory = new ConsoleOneLineLoggerFactory();
            var progressBarReporter =
                new ProgressBarReporter(consoleOneLineLoggerFactory.Create(), new StopWatchProvider());
            var mutantKilledLogger = consoleOneLineLoggerFactory.Create();
            var mutantSurvivedLogger = consoleOneLineLoggerFactory.Create();
            var mutantTimeoutLogger = consoleOneLineLoggerFactory.Create();

            var mutantsResultReporter = new MutantsResultReporter(
                mutantKilledLogger,
                mutantSurvivedLogger,
                mutantTimeoutLogger);

            return new ProgressReporter(mutantsResultReporter, progressBarReporter);
        }
    }
}
