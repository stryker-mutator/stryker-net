using System.Collections.Generic;
using Stryker.Abstractions.Reporting;
using Stryker.Reporters.Json.SourceFiles;

namespace Stryker.Abstractions.UnitTest.Reporters.Json;

public class MockJsonReportFileComponent : SourceFile
{
    public MockJsonReportFileComponent(
        string language,
        string source,
        ISet<IJsonMutant> mutants
    )
    {
        Language = language;
        Source = source;
        Mutants = mutants;
    }
}
