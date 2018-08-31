using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        int InitialTest(ITestRunner testRunner);
    }
}