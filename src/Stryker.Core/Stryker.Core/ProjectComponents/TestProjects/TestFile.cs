using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestFile :IEquatable<TestFile>
    {
        public string FilePath { get; set; }
        public string Source { get; set; }
        public ISet<TestCase> Tests { get; set; } = new HashSet<TestCase>();

        public static TestFile operator +(TestFile a, TestFile b)
        {
            if(a.Equals(b))
            {
                a.Tests.UnionWith(b.Tests);
            }
            return a;
        }

        public bool Equals(TestFile other) => other.FilePath.Equals(FilePath) && other.Source.Equals(Source);

        public override bool Equals(object other)
        {
            if (other is TestFile file)
            {
                return Equals(file);
            }
            return false;
        }

        public override int GetHashCode() => FilePath.GetHashCode() ^ Source.GetHashCode();
    }
}
