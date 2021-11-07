using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class JsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<JsonTest> Tests { get; set; }

        public JsonTestFile(TestFile testFile)
        {
            Source = testFile.Source;
            Language = "cs";
            Tests = new HashSet<JsonTest>();

            foreach (var test in testFile.Tests)
            {
                Tests.Add(new JsonTest
                {
                    Id = test.Id.ToString(),
                    Name = test.Name
                });
            }
        }
    }
}
