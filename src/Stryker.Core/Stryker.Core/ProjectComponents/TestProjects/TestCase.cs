using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.ProjectComponents.TestProjects
{
    public sealed class TestCase : IEquatable<ITestCase>, ITestCase
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public SyntaxNode Node { get; init; }

        public bool Equals(ITestCase other) => other.Id == Id && other.Name == Name && other.Node.Span == Node.Span;

        public override bool Equals(object obj) => obj is ITestCase @case && Equals(@case);

        public override int GetHashCode() => (Id, Name).GetHashCode();
    }
}
