using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Stryker.Core.Reporters
{
    public static class ReporterFactory
    {
        public static IReporter Create(string type)
        {
            switch (type.ToLower())
            {
                case "rapportonly":
                    return new ConsoleRapportReporter();
                default:
                    return new BroadcastReporter(new Collection<IReporter>()
                    {
                        new ConsoleRapportReporter(),
                        new ConsoleDotProgressReporter(),
                        new ConsoleStatusReporter()
                    });
            }
        }
    }
}
