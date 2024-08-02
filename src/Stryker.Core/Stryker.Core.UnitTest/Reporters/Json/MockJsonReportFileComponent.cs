using System.Collections.Generic;
using Stryker.Configuration.Reporters.Json.SourceFiles;

namespace Stryker.Configuration.UnitTest.Reporters.Json;

public class MockJsonReportFileComponent : SourceFile
{
    public MockJsonReportFileComponent(
        string language,
        string source,
        ISet<JsonMutant> mutants
    )
    {
        Language = language;
        Source = source;
        Mutants = mutants;
    }
}
