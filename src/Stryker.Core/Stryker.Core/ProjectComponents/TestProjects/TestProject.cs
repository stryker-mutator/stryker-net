using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Initialisation.SolutionAnalyzer;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestProject : IEquatable<TestProject>
    {
        public IAnalyzerResult TestProjectAnalyzerResult { get; init; }
        public IEnumerable<TestFile> TestFiles { get; init; } = new List<TestFile>();

        public bool Equals(TestProject other) => other.TestProjectAnalyzerResult.Equals(TestProjectAnalyzerResult) && other.TestFiles.SequenceEqual(TestFiles);

        public override bool Equals(object obj) => obj is TestProject project && Equals(project);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => TestProjectAnalyzerResult.GetHashCode();
    }
}
