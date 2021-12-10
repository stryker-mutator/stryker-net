using System;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestCase : IEquatable<TestCase>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public int Line { get; init; }
        public string Source { get; init; }

        public bool Equals(TestCase other) => other.Id.Equals(Id) && other.Name.Equals(Name) && other.Line.Equals(Line) && other.Source.Equals(Source);

        public override bool Equals(object obj) => obj is TestCase @case && Equals(@case);

        public override int GetHashCode() => (Id, Name, Line).GetHashCode() ^ Source?.GetHashCode() ?? 555;
    }
}
