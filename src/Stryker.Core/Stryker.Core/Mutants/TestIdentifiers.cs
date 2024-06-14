using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Shared.Tests;

namespace Stryker.Core.Mutants;
public class TestIdentifiers : ITestIdentifiers
{
    private readonly HashSet<Identifier> _identifiers;

    private static readonly TestIdentifiers everyTests = new();
    private static readonly TestIdentifiers noTestAtAll = new(Array.Empty<Identifier>());

    private TestIdentifiers() => _identifiers = null;

    public TestIdentifiers(IEnumerable<ITestDescription> testDescriptions) : this(testDescriptions.Select(t => t.Id))
    { }

    public TestIdentifiers(IEnumerable<Identifier> identifiers) => _identifiers = new HashSet<Identifier>(identifiers);

    public TestIdentifiers(HashSet<Identifier> identifiers) => _identifiers = identifiers;

    public TestIdentifiers(HashSet<Guid> set) => _identifiers = set.Select(Identifier.Create).ToHashSet();

    public TestIdentifiers(IEnumerable<Guid> guids) => _identifiers = guids is not null ? new HashSet<Identifier>(guids.Select(Identifier.Create)) : null;

    public TestIdentifiers(params Guid[] guids) : this((IEnumerable<Guid>)guids)
    { }

    public bool IsEmpty => _identifiers is { Count: 0 };

    public bool IsEveryTest => _identifiers is null;

    public int Count => _identifiers?.Count ?? 0;

    public ITestIdentifiers Merge(ITestIdentifiers other)
    {
        if (IsEveryTest)
        {
            return this;
        }

        var result = new HashSet<Identifier>(_identifiers ?? []);
        result.UnionWith(other.GetIdentifiers());
        return new TestIdentifiers(result);
    }

    public bool Contains(Identifier testId) => IsEveryTest || _identifiers?.Contains(testId) is true;

    public bool IsIncludedIn(ITestIdentifiers other) => other.IsEveryTest || _identifiers?.IsSubsetOf(other.GetIdentifiers()) is true;

    public ITestIdentifiers Excluding(ITestIdentifiers testsToSkip)
    {
        if (IsEmpty || testsToSkip.IsEmpty)
        {
            return this;
        }

        if (IsEveryTest)
        {
            throw new InvalidOperationException("Can't exclude from EveryTest");
        }

        return testsToSkip.IsEveryTest ? NoTest() : new TestIdentifiers(_identifiers.Except(((TestIdentifiers)testsToSkip)._identifiers));
    }

    public static TestIdentifiers EveryTest() => everyTests;

    public static TestIdentifiers NoTest() => noTestAtAll;

    public IEnumerable<Identifier> GetIdentifiers() => _identifiers ?? [];

    public bool ContainsAny(ITestIdentifiers other)
    {
        if (other.IsEmpty || IsEmpty)
        {
            return false;
        }
        if (IsEveryTest || other.IsEveryTest)
        {
            return true;
        }

        return GetIdentifiers().Intersect(other.GetIdentifiers()).Any();
    }

    public ITestIdentifiers Intersect(ITestIdentifiers other) => IsEveryTest ? new TestIdentifiers(other.GetIdentifiers()) : new TestIdentifiers(GetIdentifiers().Intersect(other.GetIdentifiers()));
}
