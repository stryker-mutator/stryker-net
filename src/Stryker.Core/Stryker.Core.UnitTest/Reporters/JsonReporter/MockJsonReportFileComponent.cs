using System.Collections.Generic;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MockJsonReportFileComponent : JsonSourceFile
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
