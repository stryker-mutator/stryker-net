using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ReporterFactoryTests
    {
        [Fact]
        public void ReporterFactory_CreatesRequestedReporters()
        {
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "Json" }));
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldHaveSingleItem().ShouldBeOfType(typeof(JsonReporter));
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "All" }));
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldContain(r => r is JsonReporter);
            result.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            result.Reporters.ShouldContain(r => r is ConsoleReportReporter);
            result.Reporters.ShouldContain(r => r is ProgressReporter);
        }

        [Fact]
        public void ReporterFactory_CreatesReplacementsForDeprecatedReporterOptions()
        {
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "ConsoleProgressBar", "ConsoleProgressDots", "ConsoleReport" }));
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            result.Reporters.ShouldContain(r => r is ConsoleReportReporter);
            result.Reporters.ShouldContain(r => r is ProgressReporter);
        }
    }
}
