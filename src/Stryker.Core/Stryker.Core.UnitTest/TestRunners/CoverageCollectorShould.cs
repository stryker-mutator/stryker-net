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
    // mock for the actual MutantControl class injected in the mutated assembly.
    // used for unit test
    public static class MutantControl
    {
        public static bool CaptureCoverage;
        public static int ActiveMutant = -1;
        public static IList<int>[] GetCoverageData()
        {
            return new List<int>[2];
        }
    }

    public class CoverageCollectorShould
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

            var mutantMap = new List<(int, IEnumerable<Guid>)>() {(0, new List<Guid>())};

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(false, mutantMap, this.GetType().Namespace)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);

            collector.TestCaseStart(new TestCaseStartArgs(new TestCase("theTest", new Uri("xunit://"), "source.cs")));

            MutantControl.ActiveMutant.ShouldBe(0);
        }
    }
}
