using System.Collections.Generic;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MockJsonReport : JsonReport
    {
        public MockJsonReport(
            IDictionary<string, int> thresholds,
            IDictionary<string, JsonReportFileComponent> files
        )
        {
            SchemaVersion = "1.3";
            Thresholds = thresholds;
            Files = files;
        }
    }
}
