using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestCase : IEquatable<TestCase>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public SyntaxNode Node { get; init; }

        public bool Equals(TestCase other) => other.Id == Id && other.Name == Name && other.Node.Span == Node.Span;

        public override bool Equals(object obj) => obj is TestCase @case && Equals(@case);

        public override int GetHashCode() => (Id, Name).GetHashCode();
    }
}
