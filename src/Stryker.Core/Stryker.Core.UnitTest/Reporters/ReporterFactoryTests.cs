using System;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ReporterFactoryTests : TestBase
    {
        private Mock<IGitInfoProvider> _branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

        [Theory]
        [InlineData(Reporter.Json, typeof(JsonReporter))]
        [InlineData(Reporter.Html, typeof(HtmlReporter))]
        [InlineData(Reporter.Progress, typeof(ProgressReporter))]
        [InlineData(Reporter.Dots, typeof(ConsoleDotProgressReporter))]
        [InlineData(Reporter.ClearText, typeof(ClearTextReporter))]
        public void ReporterFactory_CreatesRequestedReporters(Reporter option, Type reporter)
        {
            var target = new ReporterFactory();
            var options = new StrykerOptions { Reporters = new[] { option } };

            var result = target.Create(options, _branchProviderMock.Object);
            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            var target = new ReporterFactory();
            var options = new StrykerOptions { Reporters = new[] { Reporter.All } };

            var result = (BroadcastReporter)target.Create(options, _branchProviderMock.Object);

            var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
            broadcastReporter.Reporters.ShouldContain(r => r is JsonReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ClearTextTreeReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is ProgressReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is DashboardReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is MarkdownSummaryReporter);
            broadcastReporter.Reporters.ShouldContain(r => r is BaselineReporter);

            result.Reporters.Count().ShouldBe(9);
        }
    }
}
