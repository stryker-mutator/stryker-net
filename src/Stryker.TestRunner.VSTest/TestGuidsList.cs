using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;

internal class TestGuidsList : ITestIdentifiers
{
    private readonly HashSet<Identifier>? _identifiers;

    private static readonly TestGuidsList everyTests = new();
    private static readonly TestGuidsList noTestAtAll = new(Array.Empty<Identifier>());

    private TestGuidsList() => _identifiers = null;

    public TestGuidsList(IEnumerable<ITestDescription> testDescriptions) : this(testDescriptions.Select(t => t.Id.ToGuid()))
    { }

    public TestGuidsList(IEnumerable<Identifier> identifiers) => _identifiers = new HashSet<Identifier>(identifiers);
    
    public TestGuidsList(HashSet<Identifier> identifiers) => _identifiers = identifiers;

    public TestGuidsList(HashSet<Guid> set) => _identifiers = set.Select(Identifier.Create).ToHashSet();

    public TestGuidsList(IEnumerable<Guid>? guids) => _identifiers = guids is not null ? new HashSet<Identifier>(guids.Select(Identifier.Create)) : null;

    public TestGuidsList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
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
        return new TestGuidsList(result);
    }

    public bool Contains(Identifier testId) => IsEveryTest || _identifiers?.Contains(testId) is false;

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

        return testsToSkip.IsEveryTest ? NoTest() : new TestGuidsList(_identifiers.Except(testsToSkip.GetIdentifiers()));
    }

    public static TestGuidsList EveryTest() => everyTests;

    public static TestGuidsList NoTest() => noTestAtAll;

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

    public ITestIdentifiers Intersect(ITestIdentifiers other) => IsEveryTest ? new TestGuidsList(other.GetIdentifiers()) : new TestGuidsList(GetIdentifiers().Intersect(other.GetIdentifiers()));
}
