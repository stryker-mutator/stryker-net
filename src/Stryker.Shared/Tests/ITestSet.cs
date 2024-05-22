namespace Stryker.Shared.Tests;

public interface ITestSet
{
    int Count { get; }
    ITestDescription this[Guid id] { get; }
    void RegisterTests(IEnumerable<ITestDescription> tests);
    void RegisterTest(ITestDescription test);
    IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids);
}
