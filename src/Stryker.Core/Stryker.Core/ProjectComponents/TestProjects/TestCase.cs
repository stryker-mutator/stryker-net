using System;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestCase : IEquatable<TestCase>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public int Line { get; init; }
        public string Source { get; init; }

        public bool Equals(TestCase other) => other.Id == Id && other.Name == Name && other.Line == Line && other.Source == Source;

        public override bool Equals(object obj) => obj is TestCase @case && Equals(@case);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => (Id, Name, Line).GetHashCode() ^ Source?.GetHashCode() ?? 555;
    }
}
