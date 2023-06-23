using System.Collections.Generic;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.UnitTest.Reporters.Json
{
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
}
