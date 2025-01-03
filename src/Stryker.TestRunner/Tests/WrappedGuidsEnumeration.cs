using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Tests;

public class WrappedGuidsEnumeration : ITestIdentifiers
{
    private readonly IEnumerable<Identifier> _identifiers;

    public int Count => _identifiers.Count();

    public bool IsEmpty => _identifiers is null || !_identifiers.Any();

    public bool IsEveryTest => false;

    public ITestIdentifiers Merge(ITestIdentifiers other) => MergeList(this, other);

    public bool Contains(Identifier testId) => _identifiers.Any(g => g == testId);

    public bool ContainsAny(ITestIdentifiers other) => _identifiers.Any(other.Contains);

    public bool IsIncludedIn(ITestIdentifiers other) => _identifiers.All(other.Contains);

    public WrappedGuidsEnumeration(IEnumerable<Guid> guids) => _identifiers = guids.Select(Identifier.Create);

    public WrappedGuidsEnumeration(IEnumerable<Identifier> identifiers) => _identifiers = identifiers;

    public static ITestIdentifiers MergeList(ITestIdentifiers a, ITestIdentifiers b)
    {
        if (a.GetIdentifiers() is null)
        {
            return b;
        }

        return b.GetIdentifiers() is null ? a : new WrappedGuidsEnumeration(a.GetIdentifiers().Union(b.GetIdentifiers()));
    }

    public IEnumerable<Identifier> GetIdentifiers() => _identifiers;

    public ITestIdentifiers Intersect(ITestIdentifiers other) => IsEveryTest ? new WrappedGuidsEnumeration(other.GetIdentifiers()) : new WrappedGuidsEnumeration(_identifiers.Intersect(other.GetIdentifiers()));

    public ITestIdentifiers Excluding(ITestIdentifiers testsToSkip) => throw new NotImplementedException();
}
