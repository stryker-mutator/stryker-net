using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Stryker.DataCollector;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class CoverageCollectorShould
    {
        [Fact]
        public void ProperlyCaptureParams()
        {
            CoverageCollector collector = new CoverageCollector();

            var start = new TestSessionStartArgs
            {
                Configuration = CoverageCollector.GetVsTestSettings(true, true, null)
            };
            var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
            collector.Initialize(mock.Object);

            collector.TestSessionStart(start);
        }
    }
}
    