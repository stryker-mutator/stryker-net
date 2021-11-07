using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestProjectsInfo
    {
        public ISet<TestProject> TestProjects { get; set; } = new HashSet<TestProject>();
        public ISet<TestFile> TestFiles => TestProjects.SelectMany(testProject => testProject.TestFiles).ToHashSet();

        public static TestProjectsInfo operator +(TestProjectsInfo a, TestProjectsInfo b)
        {
            a.TestProjects.UnionWith(b.TestProjects);
            return a;
        }
    }
}
