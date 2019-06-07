using System;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        int DiscoverTests();
        TestRunResult RunAll(int? timeoutMS, int? activeMutationId);
    }
}