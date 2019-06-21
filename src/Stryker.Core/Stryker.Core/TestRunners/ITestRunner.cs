using System;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        int DiscoverNumberOfTests();
        TestRunResult RunAll(int? timeoutMS, int? activeMutationId);
    }
}