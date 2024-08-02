using System.Collections.Generic;
using System.Linq;
using Stryker.Configuration.ProjectComponents.TestProjects;

namespace Stryker.Configuration.Reporters.Json.TestFiles
{
    public class JsonTestFile : IJsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<JsonTest> Tests { get; set; }

        public JsonTestFile() { }

        public JsonTestFile(TestFile testFile)
        {
            Source = testFile?.Source;
            Language = "cs";
            Tests = new HashSet<JsonTest>();

            AddTestFile(testFile);
        }

        public void AddTestFile(TestFile testFile)
        {
            foreach (var test in testFile?.Tests ?? Enumerable.Empty<TestCase>())
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
