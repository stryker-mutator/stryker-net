using System;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
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
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var target = new ReporterFactory();

            var result = target.Create(new StrykerOptions(reporters: new[] { option }), branchProviderMock.Object);
            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var target = new ReporterFactory();

            var result = (BroadcastReporter)target.Create(new StrykerOptions(reporters: new[] { "All" }), branchProviderMock.Object);

            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldContain(r => r is JsonReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextTreeReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is DashboardReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is GitBaselineReporter);

            result.Reporters.Count().ShouldBe(8);
        }

        [Fact]
        public void ReporterFactory_CreatesReplacementsForDeprecatedReporterOptions()
        {
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var target = new ReporterFactory();

            var result = target.Create(new StrykerOptions(reporters: new[] { "ConsoleProgressBar", "ConsoleProgressDots", "ConsoleReport" }), branchProvider: branchProviderMock.Object);
            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ProgressReporter);
        }
    }
}
