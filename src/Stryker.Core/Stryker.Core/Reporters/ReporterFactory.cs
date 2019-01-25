using Stryker.Core.Options;
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

        private static IDictionary<Options.Values.Reporters, IReporter> CreateReporters(StrykerOptions options)
        {
            return new Dictionary<Options.Values.Reporters, IReporter>
            {
                { Options.Values.Reporters.ConsoleProgressDots, new ConsoleDotProgressReporter() },
                { Options.Values.Reporters.ConsoleProgressBar, CreateProgressReporter() },
                { Options.Values.Reporters.ConsoleReport, new ConsoleReportReporter(options) },
                { Options.Values.Reporters.Json, new JsonReporter(options) }
            };
        }

        private static IEnumerable<IReporter> DetermineEnabledReporters(IEnumerable<Options.Values.Reporters> enabledReporters, IDictionary<Options.Values.Reporters, IReporter> possibleReporters)
        {
            return enabledReporters.Contains(Options.Values.Reporters.All) ? possibleReporters.Values :
                possibleReporters.Where((KeyValuePair<Options.Values.Reporters, IReporter> reporter) => enabledReporters.Contains(reporter.Key))
                    .Select((KeyValuePair<Options.Values.Reporters, IReporter> reporter) => reporter.Value);
        }

        private static ProgressReporter CreateProgressReporter()
        {
            var consoleOneLineLoggerFactory = new ConsoleOneLineLoggerFactory();
            var progressBarReporter =
                new ProgressBarReporter(consoleOneLineLoggerFactory.Create(), new StopWatchProvider());
            var mutantKilledLogger = consoleOneLineLoggerFactory.Create();
            var mutantSurvivedLogger = consoleOneLineLoggerFactory.Create();
            var mutantTimeoutLogger = consoleOneLineLoggerFactory.Create();
            var mutantRuntimeErrorLogger = consoleOneLineLoggerFactory.Create();

            var mutantsResultReporter = new MutantsResultReporter(
                mutantKilledLogger,
                mutantSurvivedLogger,
                mutantTimeoutLogger,
                mutantRuntimeErrorLogger);

            return new ProgressReporter(mutantsResultReporter, progressBarReporter);
        }
    }
}
