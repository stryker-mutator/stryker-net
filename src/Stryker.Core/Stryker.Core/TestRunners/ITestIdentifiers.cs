using System.Collections.Generic;

namespace Stryker.Core.TestRunners;

public interface ITestIdentifiers
{
    IEnumerable<Identifier> GetIdentifiers();
    int Count { get; }
    bool IsEmpty { get; }
    bool IsEveryTest { get; }
    ITestIdentifiers Merge(ITestIdentifiers other);
    bool Contains(Identifier testId);
    bool ContainsAny(ITestIdentifiers other);
    bool IsIncludedIn(ITestIdentifiers other);
    ITestIdentifiers Intersect(ITestIdentifiers other);
    ITestIdentifiers Excluding(ITestIdentifiers testsToSkip);
}
