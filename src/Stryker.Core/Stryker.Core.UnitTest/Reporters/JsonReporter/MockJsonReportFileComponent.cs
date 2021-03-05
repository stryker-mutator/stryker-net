using System;
using System.Collections.Generic;
using System.Text;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MockJsonReportFileComponent : JsonReportFileComponent
    {
        public MockJsonReportFileComponent(
            string language,
            string source,
            ISet<JsonMutant> mutants
        ) : base(language, source, mutants)
        {

        }
    }
}
