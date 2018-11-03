using System.Collections.ObjectModel;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Progress;

namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(StrykerOptions options)
        {
            var progressReporter = CreateProgressReporter();
            return new BroadcastReporter(new Collection<IReporter>()
            {
                new ConsoleReportReporter(options),
                progressReporter
            });
        }

        private static ProgressReporter CreateProgressReporter()
        {
            var consoleOneLineLoggerFactory = new ConsoleOneLineLoggerFactory();
            var progressBarReporter = new ProgressBarReporter(consoleOneLineLoggerFactory.Create());

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
