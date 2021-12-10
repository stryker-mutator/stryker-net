using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestFile : IEquatable<TestFile>
    {
        public string FilePath { get; init; }
        public string Source { get; init; }
        public IEnumerable<TestCase> Tests { get; set; } = new List<TestCase>();

        public static TestFile operator +(TestFile a, TestFile b)
        {
            if (a == b)
            {
                a.Tests = a.Tests.Union(b.Tests);
            }
            return a;
        }

        public bool Equals(TestFile other) => other.FilePath.Equals(FilePath) && other.Source.Equals(Source);

        public override bool Equals(object obj) => obj is TestFile file && Equals(file);

        public override int GetHashCode() => FilePath.GetHashCode() ^ Source.GetHashCode();
    }
}
