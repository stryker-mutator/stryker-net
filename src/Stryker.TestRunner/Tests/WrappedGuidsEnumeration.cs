using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Tests;

/// <summary>
/// Wraps an enumeration of test identifiers into an ITestIdentifiers implementation. This allows for event passing
/// and test results processing with limited allocation and performance overhead.
/// </summary>
/// <param name="identifiers">list of test identifiers</param>
public class WrappedIdentifierEnumeration(IEnumerable<string> identifiers) : ITestIdentifiers
{
    public int Count => identifiers.Count();

    public bool IsEmpty => !identifiers.Any();

    /// <summary>
    /// Always returns false, as this implementation is meant to represent a specific enumeration of identifiers.
    /// </summary>
    public bool IsEveryTest => false;

    public bool Contains(string testId) => identifiers.Any(g => g == testId);

    public bool ContainsAny(ITestIdentifiers other) => identifiers.Any(other.Contains);

    public bool IsIncludedIn(ITestIdentifiers other) => identifiers.All(other.Contains);

    public IEnumerable<string> GetIdentifiers() => identifiers;
}
