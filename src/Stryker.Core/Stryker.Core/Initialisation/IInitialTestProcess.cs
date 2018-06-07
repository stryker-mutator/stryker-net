using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        void InitialTest(ITestRunner testRunner);
    }
}