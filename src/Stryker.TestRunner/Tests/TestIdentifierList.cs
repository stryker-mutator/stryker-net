using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Tests;

public class TestIdentifierList : ITestIdentifiers
{
    private readonly HashSet<string>? _identifiers;

    private static readonly TestIdentifierList everyTests = new();
    private static readonly TestIdentifierList noTestAtAll = new(Array.Empty<string>());

    private TestIdentifierList() => _identifiers = null;

    public TestIdentifierList(IEnumerable<ITestDescription> testDescriptions) : this(testDescriptions.Select(t => t.Id.ToString()))
    { }

    public TestIdentifierList(IEnumerable<string> identifiers) => _identifiers = identifiers != null ? new HashSet<string>(identifiers) : null;

    public TestIdentifierList(HashSet<string> identifiers) => _identifiers = identifiers;

    public TestIdentifierList(params string[] ids) : this((IEnumerable<string>)ids)
    { }

    public static ITestIdentifiers EveryTest() => everyTests;

    public static ITestIdentifiers NoTest() => noTestAtAll;

    public int Count => _identifiers?.Count ?? 0;

    public bool IsEmpty => _identifiers is { Count: 0 };

    public bool IsEveryTest => _identifiers is null;

    public void AddIdentifier(string identifier)
    {
        _identifiers?.Add(identifier);
    }

    public IEnumerable<string> GetIdentifiers() => _identifiers ?? [];

    public bool Contains(string testId) => IsEveryTest || _identifiers?.Contains(testId) is false;

    public ITestIdentifiers Intersect(ITestIdentifiers other) => IsEveryTest ? new TestIdentifierList(other.GetIdentifiers()) : new TestIdentifierList(GetIdentifiers().Intersect(other.GetIdentifiers()));

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

        return GetIdentifiers().Intersect(other.GetIdentifiers()).Any();
    }

    public ITestIdentifiers Merge(ITestIdentifiers other)
    {
        if (IsEveryTest)
        {
            return this;
        }

        var result = new HashSet<string>(_identifiers ?? []);
        result.UnionWith(other.GetIdentifiers());
        return new TestIdentifierList(result);
    }

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

        return testsToSkip.IsEveryTest ? NoTest() : new TestIdentifierList(_identifiers.Except(testsToSkip.GetIdentifiers()));
    }
}
