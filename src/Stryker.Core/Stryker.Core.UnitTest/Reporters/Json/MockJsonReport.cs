using System.Collections.Generic;
using Stryker.Abstractions.Reporters.Json;
using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.UnitTest.Reporters.Json
{
    public class MockJsonReport : JsonReport
    {
        public MockJsonReport(
            IDictionary<string, int> thresholds,
            IDictionary<string, ISourceFile> files
        )
        {
            Thresholds = thresholds;
            Files = files;
        }
    }
}
