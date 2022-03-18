using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestProjectsInfo
    {
        public IEnumerable<TestProject> TestProjects { get; set; }
        public IEnumerable<TestFile> TestFiles => TestProjects.SelectMany(testProject => testProject.TestFiles);

        public static TestProjectsInfo operator +(TestProjectsInfo a, TestProjectsInfo b)
        {
            a.TestProjects = a.TestProjects.Union(b.TestProjects);
            return a;
        }
    }
}
