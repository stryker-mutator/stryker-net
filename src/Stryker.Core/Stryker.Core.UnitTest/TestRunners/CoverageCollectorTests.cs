using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Shouldly;
using Stryker.DataCollector;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners;

// mock for the actual MutantControl class injected in the mutated assembly.
// used for unit test
public static class MutantControl
{
    public static bool CaptureCoverage;
    public static int ActiveMutant = -1;
    private static List<int>[] coverageData = { new List<int>(), new List<int>() };
    public static IList<int>[] GetCoverageData()
    {
        var result = coverageData;
        ClearCoverageInfo();
        return result;
    }

    public static void ClearCoverageInfo() => coverageData = new[] { new List<int>(), new List<int>() };

    public static void HitNormal(int mutation) => coverageData[0].Add(mutation);

    public static void HitStatic(int mutation) => coverageData[1].Add(mutation);
}

public class CoverageCollectorTests : TestBase
{
    [Fact]
    public void ProperlyCaptureParams()
    {
        var collector = new CoverageCollector();

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(true, null, "Stryker.Core.UnitTest.TestRunners")
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);
        collector.TestCaseStart(new TestCaseStartArgs(new TestCase("theTest", new Uri("xunit://"), "source.cs")));
        MutantControl.CaptureCoverage.ShouldBeTrue();
    }

    [Fact]
    public void ProperlySelectMutant()
    {
        var collector = new CoverageCollector();

        var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
        var nonCoveringTestCase = new TestCase("theOtherTest", new Uri("xunit://"), "source.cs");
        var mutantMap = new List<(int, IEnumerable<Guid>)> {(10, new List<Guid>{testCase.Id}), (5, new List<Guid>{nonCoveringTestCase.Id})};

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, this.GetType().Namespace)
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);
        MutantControl.ActiveMutant.ShouldBe(-1);

        collector.TestCaseStart(new TestCaseStartArgs(testCase));

        MutantControl.ActiveMutant.ShouldBe(10);
    }

    [Fact]
    public void SelectMutantEarlyIfSingle()
    {
        var collector = new CoverageCollector();

        var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
        var nonCoveringTestCase = new TestCase("theOtherTest", new Uri("xunit://"), "source.cs");
        var mutantMap = new List<(int, IEnumerable<Guid>)> {(5, new List<Guid>{testCase.Id})};

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, this.GetType().Namespace)
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);

        MutantControl.ActiveMutant.ShouldBe(5);
    }

    [Fact]
    public void ProperlyCaptureCoverage()
    {
        var collector = new CoverageCollector();

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(true, null, "Stryker.Core.UnitTest.TestRunners")
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);
        var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
        collector.TestCaseStart(new TestCaseStartArgs(testCase));
        MutantControl.HitNormal(0);
        MutantControl.HitNormal(1);
        MutantControl.HitStatic(1);
        var dataCollection = new DataCollectionContext(testCase);
        collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));

        mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.PropertyName, "0,1;1"), Times.Once);
        collector.TestSessionEnd(new TestSessionEndArgs());
    }

    [Fact]
    public void ProperlyReportNoCoverage()
    {
        var collector = new CoverageCollector();

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(true, null, "Stryker.Core.UnitTest.TestRunners")
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);
        var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
        collector.TestCaseStart(new TestCaseStartArgs(testCase));
        var dataCollection = new DataCollectionContext(testCase);
        collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));

        mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.PropertyName, ";"), Times.Once);
        collector.TestSessionEnd(new TestSessionEndArgs());
    }

    [Fact]
    public void ProperlyReportLeakedMutations()
    {
        var collector = new CoverageCollector();

        var start = new TestSessionStartArgs
        {
            Configuration = CoverageCollector.GetVsTestSettings(true, null, "Stryker.Core.UnitTest.TestRunners")
        };
        var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

        collector.Initialize(mock.Object);

        collector.TestSessionStart(start);
        var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
        MutantControl.HitNormal(0);
        collector.TestCaseStart(new TestCaseStartArgs(testCase));
        var dataCollection = new DataCollectionContext(testCase);
        MutantControl.HitNormal(1);
        collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));

        mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.PropertyName, "1;"), Times.Once);
        mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.OutOfTestsPropertyName, "0"), Times.Once);
        collector.TestSessionEnd(new TestSessionEndArgs());
    }

}
