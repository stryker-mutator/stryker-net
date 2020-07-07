﻿using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
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
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { option }), branchProviderMock.Object);
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldHaveSingleItem().ShouldBeOfType(reporter);
        }

        [Fact]
        public void ReporterFactory_CreatesAllReporters()
        {
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "All" }), branchProviderMock.Object);
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldContain(r => r is JsonReporter);
            result.Reporters.ShouldContain(r => r is HtmlReporter);
            result.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            result.Reporters.ShouldContain(r => r is ClearTextReporter);
            result.Reporters.ShouldContain(r => r is ProgressReporter);
            result.Reporters.ShouldContain(r => r is DashboardReporter);
            result.Reporters.ShouldContain(r => r is GitBaselineReporter);

            result.Reporters.Count().ShouldBe(7);
        }

        [Fact]
        public void ReporterFactory_CreatesReplacementsForDeprecatedReporterOptions()
        {
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            BroadcastReporter result = (BroadcastReporter)ReporterFactory.Create(new StrykerOptions(reporters: new[] { "ConsoleProgressBar", "ConsoleProgressDots", "ConsoleReport" }), branchProvider: branchProviderMock.Object);
            result.ShouldBeOfType(typeof(BroadcastReporter));
            result.Reporters.ShouldContain(r => r is ConsoleDotProgressReporter);
            result.Reporters.ShouldContain(r => r is ClearTextReporter);
            result.Reporters.ShouldContain(r => r is ProgressReporter);
        }
    }
}
