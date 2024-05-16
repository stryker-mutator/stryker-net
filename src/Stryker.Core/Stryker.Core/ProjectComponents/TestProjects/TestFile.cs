using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.ProjectComponents.TestProjects;

public sealed class TestFile : IEquatable<TestFile>
{
    public SyntaxTree SyntaxTree { get; init; }
    public string FilePath { get; init; }
    public string Source { get; init; }
    public IEnumerable<TestCase> Tests { get; private set; } = new List<TestCase>();

    public void AddTest(Guid id, string name, SyntaxNode node)
    {
        if (Tests.Any(test => test.Id == id))
        {
            return;
        }

        ((IList<TestCase>)Tests).Add(new TestCase
        {
            Id = id,
            Name = name,
            Node = node
        });
    }

    public bool Equals(TestFile other) => other!=null && other.FilePath.Equals(FilePath) && other.Source.Equals(Source);

    public override bool Equals(object obj) => obj is TestFile file && Equals(file);

    // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
    public override int GetHashCode() => FilePath.GetHashCode() ^ Source.GetHashCode();
}
