using System.Collections.Generic;

namespace Stryker.Abstractions.Testing;

public interface ITestSet
{
    int Count { get; }
    ITestDescription this[Identifier id] { get; }
    void RegisterTests(IEnumerable<ITestDescription> tests);
    void RegisterTest(ITestDescription test);
    IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids);
}
