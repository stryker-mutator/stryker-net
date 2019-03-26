using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;

namespace Stryker.DataCollector
{

    [DataCollectorFriendlyName("StrykerCoverage")]
    [DataCollectorTypeUri("https://stryker-mutator.io/")]
    public class CoverageCollector: InProcDataCollection
    {
        private IDataCollectionSink dataSink;
        private const string TemplateForConfiguration = @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0} >
        <Configuration>
        </Configuration>
        </InProcDataCollector>
        </InProcDataCollectors>
        </InProcDataCollectionRunSettings>";


        public static string GetVsTestSettings()
        {
            var codeBase = typeof(CoverageCollector).Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>().FriendlyName;
            var uri = (typeof(CoverageCollector).GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).First() as
                DataCollectorTypeUriAttribute).TypeUri;
            var line= $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            return string.Format(TemplateForConfiguration, line);
        }

        public CoverageCollector()
        {}

        public void Initialize(IDataCollectionSink dataCollectionSink)
        {
            this.dataSink = dataCollectionSink;
        }

        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {
        }

        public void TestCaseStart(TestCaseStartArgs testCaseStartArgs)
        {
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            var coverageString = Environment.GetEnvironmentVariable("Coverage");
            if (coverageString != null)
            {
                this.dataSink.SendData(testCaseEndArgs.DataCollectionContext, "Stryker.Coverage", coverageString);
                Environment.SetEnvironmentVariable("CoverageReset", "true");
            }
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
        {
        }
    }
}
