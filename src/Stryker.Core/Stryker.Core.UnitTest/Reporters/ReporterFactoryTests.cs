using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
using System;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ReporterFactoryTests
    {
        [Theory]
        [InlineData("Json", typeof(JsonReporter))]
        [InlineData("Html", typeof(HtmlReporter))]
        [InlineData("Progress", typeof(ProgressReporter))]
        [InlineData("Dots", typeof(ConsoleDotProgressReporter))]
        [InlineData("ClearText", typeof(ConsoleReportReporter))]
        public void ReporterFactory_CreatesRequestedReporters(string option, Type reporter)
        {
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { option }));
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "All" }));
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldContain(r => r is JsonReporter);
            result.Reporters.ShouldContain(r => r is HtmlReporter);
            result.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            result.Reporters.ShouldContain(r => r is ConsoleReportReporter);
            result.Reporters.ShouldContain(r => r is ProgressReporter);

            result.Reporters.Count().ShouldBe(5);
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
