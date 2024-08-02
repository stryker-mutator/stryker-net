using System.Collections.Generic;
using Stryker.Configuration.Reporters.Json;
using Stryker.Configuration.Reporters.Json.SourceFiles;

namespace Stryker.Configuration.UnitTest.Reporters.Json
{
    public class MockJsonReport : JsonReport
    {
        public MockJsonReport(
            IDictionary<string, int> thresholds,
            IDictionary<string, SourceFile> files
        )
        {
            Thresholds = thresholds;
            Files = files;
        }
    }
}
