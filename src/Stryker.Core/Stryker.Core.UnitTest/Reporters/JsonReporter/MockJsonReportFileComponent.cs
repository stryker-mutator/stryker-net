using System.Collections.Generic;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MockJsonReportFileComponent : JsonReportFileComponent
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
