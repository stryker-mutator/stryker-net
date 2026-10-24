using System.Collections.Generic;

namespace Stryker.Abstractions.Testing;


public interface ITestIdentifiers
{
    IEnumerable<string> GetIdentifiers();
    int Count { get; }
    bool IsEmpty { get; }
    bool IsEveryTest { get; }
    bool Contains(string testId);
}

public interface ITestIdentifierSet : ITestIdentifiers
{
    ITestIdentifierSet Merge(ITestIdentifiers other);
    ITestIdentifierSet Intersect(ITestIdentifiers other);
    ITestIdentifierSet Excluding(ITestIdentifiers testsToSkip);
    bool IsIncludedIn(ITestIdentifiers other);
    bool ContainsAny(ITestIdentifiers other);
}
