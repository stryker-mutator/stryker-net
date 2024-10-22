using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Progress;

namespace Stryker.Core.UnitTest.Reporters;

[TestClass]
public class ReporterFactoryTests : TestBase
{
    private Mock<IGitInfoProvider> _branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

    [TestMethod]
    [DataRow(Reporter.Json, typeof(JsonReporter))]
    [DataRow(Reporter.Html, typeof(HtmlReporter))]
    [DataRow(Reporter.Progress, typeof(ProgressReporter))]
    [DataRow(Reporter.Dots, typeof(ConsoleDotProgressReporter))]
    [DataRow(Reporter.ClearText, typeof(ClearTextReporter))]
    public void ReporterFactory_CreatesRequestedReporters(Reporter option, Type reporter)
    {
        var target = new ReporterFactory();
        var options = new StrykerOptions { Reporters = new[] { option } };

        var result = target.Create(options, _branchProviderMock.Object);
        var broadcastReporter = result.ShouldBeOfType<BroadcastReporter>();
        broadcastReporter.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
    }

    [TestMethod]
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

        result.Reporters.Count().ShouldBe(10);
    }
}
