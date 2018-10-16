using System.Collections.ObjectModel;
using Stryker.Core.Options;

namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(StrykerOptions options)
        {
            return new BroadcastReporter(new Collection<IReporter>()
            {
                new ConsoleReportReporter(options),
                new ConsoleDotProgressReporter()
            });
        }
    }
}
