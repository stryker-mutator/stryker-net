using System.Collections.Generic;

namespace Stryker.Core.TestRunners;

public interface ITestSet
{
    int Count { get; }
    ITestDescription this[Identifier id] { get; }
    void RegisterTests(IEnumerable<ITestDescription> tests);
    void RegisterTest(ITestDescription test);
    IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids);
}
