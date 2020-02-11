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
    public class CoverageCollectorShould
    {
        [Fact]
        public void ProperlyCaptureParams()
        {
            var env = new MockEnvironmentHandler();
            var collector = new CoverageCollector(env);

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(true, true, null)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
            env.Variables.ShouldContainKey("CaptureCoverage");
            env.Variables["CaptureCoverage"].ShouldBe("pipe");
        }

        [Fact]
        public void ProperlySelectMutant()
        {
            var env = new MockEnvironmentHandler();
            var collector = new CoverageCollector(env);

            var mutantMap = new Dictionary<int, IList<string>>() {[0] = new List<string>()};

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(true, true, mutantMap)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);

            collector.TestCaseStart(new TestCaseStartArgs(new TestCase("theTest", new Uri("xunit://"), "source.cs")));

            env.Variables.ShouldContainKey("ActiveMutation");
            env.Variables["ActiveMutation"].ShouldBe("0");
        }
    }

    internal class MockEnvironmentHandler : IEnvironmentVariablesHandler
    {
        public readonly Dictionary<string, string> Variables = new Dictionary<string, string>();

        public void SetVariable(string name, string value)
        {
            Variables[name] = value;
        }
    }
}
    