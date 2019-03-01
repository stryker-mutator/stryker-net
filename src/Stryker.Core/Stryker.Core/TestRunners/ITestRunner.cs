using System;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMS, int? activeMutationId);
    }
}