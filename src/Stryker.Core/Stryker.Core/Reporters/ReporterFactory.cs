using Stryker.Core.Options;
using Stryker.Core.Reporters.Progress;
using System.Collections.ObjectModel;

namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(StrykerOptions options)
        {
            var progressReporter = CreateProgressReporter();
            Collection<IReporter> reporters = new Collection<IReporter>
            {
                new ConsoleReportReporter(options),
                progressReporter,
            };
            foreach (var reporter in options.Reporters)
            {
                if (reporter == "Json")
                {
                    reporters.Add(new JsonReporter(options));
                }
            }

            return new BroadcastReporter(reporters);
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
