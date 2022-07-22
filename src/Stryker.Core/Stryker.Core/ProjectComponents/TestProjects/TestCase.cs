using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestCase : IEquatable<TestCase>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public SyntaxNode Node { get; init; }

        public bool Equals(TestCase other) => other.Id == Id && other.Name == Name;

        public override bool Equals(object obj) => obj is TestCase @case && Equals(@case);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => (Id, Name).GetHashCode();
    }
}
