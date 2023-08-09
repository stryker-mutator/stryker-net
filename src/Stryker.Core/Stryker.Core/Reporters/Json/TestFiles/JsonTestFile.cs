using System.Collections.Generic;
using System.Linq;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters.Json.TestFiles
{
    public class JsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<Test> Tests { get; set; }

        public JsonTestFile() { }

        public JsonTestFile(TestFile testFile)
        {
            Source = testFile?.Source;
            Language = "cs";
            Tests = new HashSet<Test>();

            AddTestFile(testFile);
        }

        public void AddTestFile(TestFile testFile)
        {
            foreach (var test in testFile?.Tests ?? Enumerable.Empty<TestCase>())
            {
                var span = test.Node.GetLocation().GetMappedLineSpan();
                Tests.Add(new Test(test.Id.ToString())
                {
                    Name = test.Name,
                    Location = new Location(new LocationDimensions
                    {
                        StartLine = span.StartLinePosition.Line,
                        StartCharacter = span.StartLinePosition.Character,
                        EndLine = span.EndLinePosition.Line,
                        EndCharacter = span.EndLinePosition.Character
                    })
                });
            }
        }
    }
}
