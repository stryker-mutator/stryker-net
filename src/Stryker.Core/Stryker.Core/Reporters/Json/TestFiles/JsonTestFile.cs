using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters.Json.TestFiles
{
    public class JsonTestFile : IJsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<IJsonTest> Tests { get; set; }

        public JsonTestFile() { }

        public JsonTestFile(ITestFile testFile)
        {
            Source = testFile?.Source;
            Language = "cs";
            Tests = new HashSet<IJsonTest>();

            AddTestFile(testFile);
        }

        public void AddTestFile(ITestFile testFile)
        {
            foreach (var test in testFile?.Tests ?? Enumerable.Empty<ITestCase>())
            {
                Tests.Add(new JsonTest(test.Id.ToString())
                {
                    Name = test.Name,
                    Location = new Location(test.Node.GetLocation().GetMappedLineSpan())
                });
            }
        }
    }
}
