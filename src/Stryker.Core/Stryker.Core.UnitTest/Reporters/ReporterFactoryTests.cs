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
        [InlineData("ClearText", typeof(ClearTextReporter))]
        public void ReporterFactory_CreatesRequestedReporters(string option, Type reporter)
        {
            var target = new ReporterFactory();
            
            var result = target.Create(new StrykerOptions(reporters: new[] { option }));

            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            var target = new ReporterFactory();

            var result = target.Create(new StrykerOptions(reporters: new[] { "All" }));

            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldContain(r => r is JsonReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is HtmlReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is DashboardReporter);

            broadcastReporter.Reporters.Count().ShouldBe(6);
        }

        [Fact]
        public void ReporterFactory_CreatesReplacementsForDeprecatedReporterOptions()
        {
            var target = new ReporterFactory();

            var result = target.Create(new StrykerOptions(reporters: new[] {
                "ConsoleProgressBar",
                "ConsoleProgressDots",
                "ConsoleReport"
            }));
            
            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ProgressReporter);
        }
    }
}
