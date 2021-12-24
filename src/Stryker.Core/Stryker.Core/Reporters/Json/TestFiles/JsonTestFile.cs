using System.Collections.Generic;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters.Json.TestFiles
{
    public class JsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<Test> Tests { get; set; }

        public JsonTestFile(TestFile testFile)
        {
            Source = testFile.Source;
            Language = "cs";
            Tests = new HashSet<Test>();

            foreach (var test in testFile.Tests)
            {
                Tests.Add(new Test
                {
                    Id = test.Id.ToString(),
                    Name = test.Name
                });
            }
        }
    }
}
