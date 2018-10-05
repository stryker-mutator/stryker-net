using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Stryker.Core.Options;
namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(StrykerOptions options)
        {
            switch (options.Reporter.ToLower())
            {
                case "reportonly":
                    return new ConsoleReportReporter(options);
                default:
                    return new BroadcastReporter(new Collection<IReporter>()
                    {
                        new ConsoleReportReporter(options),
                        new ConsoleDotProgressReporter(),
                        new ConsoleStatusReporter()
                    });
            }
        }
    }
}
