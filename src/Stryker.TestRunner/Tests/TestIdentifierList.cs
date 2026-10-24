using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Tests;

    public class TestIdentifierList : ITestIdentifierSet
{
    private readonly HashSet<string>? _identifiers;

    private static readonly TestIdentifierList everyTests = new();
    private static readonly TestIdentifierList noTestAtAll = new(Array.Empty<string>());

    private TestIdentifierList() => _identifiers = null;

    public TestIdentifierList(IEnumerable<ITestDescription>? testDescriptions) : this(testDescriptions?.Select(t => t.Id.ToString()))
    { }

    public TestIdentifierList(IEnumerable<string>? identifiers) => _identifiers = identifiers != null ? new HashSet<string>(identifiers)
        : null;

    public TestIdentifierList(ITestIdentifiers other) : this(other.GetIdentifiers())
    { }

    public TestIdentifierList(HashSet<string> identifiers) => _identifiers = identifiers;

    public TestIdentifierList(params string[] ids) : this((IEnumerable<string>)ids)
    { }

    public static ITestIdentifierSet EveryTest() => everyTests;

    public static ITestIdentifierSet NoTest() => noTestAtAll;

    public int Count => _identifiers?.Count ?? 0;

    public bool IsEmpty => _identifiers is { Count: 0 };

    public bool IsEveryTest => _identifiers is null;

    public IEnumerable<string> GetIdentifiers() => _identifiers;

    public bool Contains(string testId) => IsEveryTest || _identifiers?.Contains(testId) is true;

    private static ITestIdentifierSet Convert(ITestIdentifiers other)
        => other is ITestIdentifierSet set ? set : new TestIdentifierList(other);

    public ITestIdentifierSet Intersect(ITestIdentifiers other)
    {
        if (IsEveryTest)
        {
            return Convert(other);
        }
        if (other.IsEveryTest)
        {
            return this;
        }
        var intersection = new HashSet<string>(_identifiers!);
        intersection.IntersectWith(other.GetIdentifiers());
        return new TestIdentifierList(intersection);
    }

    public bool IsIncludedIn(ITestIdentifiers other) => other.IsEveryTest || _identifiers?.IsSubsetOf(other.GetIdentifiers()) is true;

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

        return _identifiers!.Overlaps(other.GetIdentifiers());
    }

    public ITestIdentifierSet Merge(ITestIdentifiers other)
    {
        if (IsEveryTest)
        {
            return this;
        }
        if (other.IsEveryTest)
        {
            return Convert(other);
        }

        var result = new HashSet<string>(_identifiers!);
        result.UnionWith(other.GetIdentifiers());
        return new TestIdentifierList(result);
    }

    public ITestIdentifierSet Excluding(ITestIdentifiers testsToSkip)
    {
        if (IsEmpty || testsToSkip.IsEmpty)
        {
            return this;
        }

        if (IsEveryTest)
        {
            throw new InvalidOperationException("Can't exclude from EveryTest");
        }

        return testsToSkip.IsEveryTest ? NoTest() : new TestIdentifierList(_identifiers.Except(testsToSkip.GetIdentifiers()));
    }
}
