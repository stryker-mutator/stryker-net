using System.Collections.Generic;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters.Json
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
