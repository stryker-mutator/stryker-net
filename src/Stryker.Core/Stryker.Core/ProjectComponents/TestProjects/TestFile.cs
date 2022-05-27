using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestFile : IEquatable<TestFile>
    {
        public string FilePath { get; init; }
        public string Source { get; init; }
        public IEnumerable<TestCase> Tests { get; private set; } = new List<TestCase>();

        public void AddTest(Guid id, string name, string sourceCode, int lineNumber)
        {
            if (Tests.Any(test => test.Id == id))
            {
                return;
            }

            var tests = new List<TestCase>(Tests);
            var test = new TestCase
            {
                Id = id,
                Name = name,
                Source = source,
                Line = lineNumber
            };

            tests.Add(test);
            Tests = tests;
        }

        public bool Equals(TestFile other) => other.FilePath.Equals(FilePath) && other.Source.Equals(Source);

        public override bool Equals(object obj) => obj is TestFile file && Equals(file);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => FilePath.GetHashCode() ^ Source.GetHashCode();
    }
}
