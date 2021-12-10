using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestFile : IEquatable<TestFile>
    {
        public string FilePath { get; init; }
        public string Source { get; init; }
        public IEnumerable<TestCase> Tests { get; set; } = new List<TestCase>();

        public bool Equals(TestFile other) => other.FilePath.Equals(FilePath) && other.Source.Equals(Source);

        public override bool Equals(object obj) => obj is TestFile file && Equals(file);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => FilePath.GetHashCode() ^ Source.GetHashCode();
    }
}
