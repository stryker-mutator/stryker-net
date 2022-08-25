using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Shouldly;
using Stryker.DataCollector;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class CoverageCollectorTests : TestBase
    {
        [Fact]
        public void ProperlyCaptureParams()
        {
            var collector = new CoverageCollector();

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(true, null, typeof(MutantControl).Namespace, false)
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
                Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, typeof(MutantControl).Namespace, false)
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
            var mutantMap = new List<(int, IEnumerable<Guid>)> {(5, new List<Guid>{testCase.Id})};

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, typeof(MutantControl).Namespace, false)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);

            MutantControl.ActiveMutant.ShouldBe(5);
        }

        [Fact]
        public void HandleSingleMutant()
        {
            var collector = new CoverageCollector();

            var mutantMap = new List<(int, IEnumerable<Guid>)> {(5, null)};

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, typeof(MutantControl).Namespace, false)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);

            MutantControl.ActiveMutant.ShouldBe(5);
        }

        [Fact]
        public void SignalMutantWasCovered()
        {
            var collector = new CoverageCollector();

            var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
            var mutantMap = new List<(int, IEnumerable<Guid>)> {(5, new List<Guid>{testCase.Id})};

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, typeof(MutantControl).Namespace, false)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
            collector.TestCaseStart(new TestCaseStartArgs(testCase));
            MutantControl.IsActive(5);
            var dataCollection = new DataCollectionContext(testCase);
            collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));
            // notify the test covered the active mutations
            mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.ActiveMutationSeen, "5"), Times.Once);
            // run a non covering test
            collector.TestCaseStart(new TestCaseStartArgs(testCase));
            collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));
            mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.ActiveMutationSeen, "5"), Times.Once);
        }

        [Fact]
        public void ProperlyCaptureCoverage()
        {
            var collector = new CoverageCollector();

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(true, null, typeof(MutantControl).Namespace, false)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
            var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
            collector.TestCaseStart(new TestCaseStartArgs(testCase));
            MutantControl.IsActive(0);
            MutantControl.IsActive(1);
            using (new MutantContext())
            {
                MutantControl.IsActive(1);
            }
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
                Configuration = CoverageCollector.GetVsTestSettings(true, null, typeof(MutantControl).Namespace, false)
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
                Configuration = CoverageCollector.GetVsTestSettings(true, null, typeof(MutantControl).Namespace, false)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
            var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
            MutantControl.IsActive(0);
            using (new MutantContext())
            {
                MutantControl.IsActive(2);
            }
            collector.TestCaseStart(new TestCaseStartArgs(testCase));
            var dataCollection = new DataCollectionContext(testCase);
            MutantControl.IsActive(1);
            collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));

            mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.PropertyName, "1;"), Times.Once);
            mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.OutOfTestsPropertyName, "0,2"), Times.Once);
            collector.TestSessionEnd(new TestSessionEndArgs());
        }


        [Fact]
        public void ProperlyCaptureTrace()
        {
            var collector = new CoverageCollector();

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(false, null, typeof(MutantControl).Namespace, true)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);

            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
            var testCase = new TestCase("theTest", new Uri("xunit://"), "source.cs");
            collector.TestCaseStart(new TestCaseStartArgs(testCase));
            MutantControl.IsActive(0);
            MutantControl.IsActive(1);
            MutantControl.IsActive(1);
            var dataCollection = new DataCollectionContext(testCase);
            collector.TestCaseEnd(new TestCaseEndArgs(dataCollection, TestOutcome.Passed));

            mock.Verify(sink => sink.SendData(dataCollection,CoverageCollector.MutationHitTrace, "0,1,1"), Times.Once);
            collector.TestSessionEnd(new TestSessionEndArgs());
        }
    }
}
