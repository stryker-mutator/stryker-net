using System;
using System.Collections.Generic;
using System.IO;
using Buildalyzer;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestProject : IEquatable<TestProject>
    {
        public IAnalyzerResult TestProjectAnalyzerResult { get; set; }
        public ISet<TestFile> TestFiles { get; set; } = new HashSet<TestFile>();

        public bool Equals(TestProject other) => other.TestProjectAnalyzerResult.Equals(TestProjectAnalyzerResult) && other.TestFiles.SetEquals(TestFiles);

        public override bool Equals(object other)
        {
            if (other is TestProject project)
            {
                return Equals(project);
            }
            return false;
        }

        public override int GetHashCode() => TestProjectAnalyzerResult.GetHashCode() ^ TestFiles.GetHashCode();
    }
}
