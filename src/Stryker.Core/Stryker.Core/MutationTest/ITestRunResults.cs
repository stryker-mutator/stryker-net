using Stryker.Core.Mutants;

namespace Stryker.Core.MutationTest
{
    public interface ITestRunResults
    {
        ITestGuids RanTests { get; }
        ITestGuids FailedTests { get; }
        ITestGuids TimedOutTests { get; }
        ITestGuids NonCoveringTests {get;}
    }
}
