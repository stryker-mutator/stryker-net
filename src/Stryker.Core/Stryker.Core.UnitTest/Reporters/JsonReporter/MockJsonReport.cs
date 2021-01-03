using System.Collections.Generic;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MockJsonReport : JsonReport
    {
        public MockJsonReport(
            IDictionary<string, int> thresholds,
            IDictionary<string, JsonReportFileComponent> files
        ) : base("1.3", thresholds, files)
        {

        }
    }
}
