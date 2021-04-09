using Microsoft.Extensions.Logging;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters
{
    public interface IReporterFactory
    {
        IReporter Create(StrykerOptions options, IGitInfoProvider branchProvider = null);
    }

    public class ReporterFactory : IReporterFactory
    {
        public IReporter Create(StrykerOptions options, IGitInfoProvider branchProvider = null)
        {
            return new BroadcastReporter(DetermineEnabledReporters(options.Reporters.ToList(), CreateReporters(options)));
        }

        private IDictionary<Reporter, IReporter> CreateReporters(IStrykerOptions options)
        {
            return new Dictionary<Reporter, IReporter>
            {
                { Reporter.Dots, new ConsoleDotProgressReporter() },
                { Reporter.Progress, CreateProgressReporter() },
                { Reporter.ClearText, new ClearTextReporter(options) },
                { Reporter.ClearTextTree, new ClearTextTreeReporter(options) },
                { Reporter.Json, new JsonReporter(options) },
                { Reporter.Html, new HtmlReporter(options) },
                { Reporter.Dashboard, new DashboardReporter(options)},
                { Reporter.Baseline, new GitBaselineReporter(options) }
            };
        }

        private IEnumerable<IReporter> DetermineEnabledReporters(IList<Reporter> enabledReporters, IDictionary<Reporter, IReporter> possibleReporters)
        {
            if (enabledReporters.Contains(Reporter.All))
            {
                return possibleReporters.Values;
            }
            var replacementFor = new Dictionary<Reporter, Reporter>
            {
                { Reporter.ConsoleProgressBar, Reporter.Progress },
                { Reporter.ConsoleProgressDots, Reporter.Dots },
                { Reporter.ConsoleReport, Reporter.ClearText }
            };
            if (enabledReporters.Where(deprecated => replacementFor.Any(replacement => replacement.Key == deprecated)).ToList() is var deprecatedReporters && deprecatedReporters.Count() > 0)
            {
                var logger = Logging.ApplicationLogging.LoggerFactory.CreateLogger(nameof(ReporterFactory));
                foreach (var deprecatedReporter in deprecatedReporters)
                {
                    logger.LogWarning($"Reporter {deprecatedReporter} is deprecated. Please use {replacementFor[deprecatedReporter]} instead.");

                    enabledReporters.Add(replacementFor[deprecatedReporter]);
                }
            }

            return possibleReporters.Where(reporter => enabledReporters.Contains(reporter.Key))
                .Select(reporter => reporter.Value);
        }

        private ProgressReporter CreateProgressReporter()
        {
            var progressBarReporter = new ProgressBarReporter(new ProgressBar(), new StopWatchProvider());

            return new ProgressReporter(progressBarReporter);
        }
    }
}
