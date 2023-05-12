namespace Stryker.Core.UnitTest.Reporters;
using System.Collections.Generic;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;

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
