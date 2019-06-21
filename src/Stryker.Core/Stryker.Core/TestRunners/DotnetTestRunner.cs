using Stryker.Core.Testing;
using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private string _path { get; set; }
        private IProcessExecutor _processExecutor { get; set; }

        public DotnetTestRunner(string path, IProcessExecutor processProxy)
        {
            _path = path;
            _processExecutor = processProxy;
        }

        public int DiscoverNumberOfTests()
        {
            throw new NotImplementedException("Dotnet test cannot return the found testcases.");
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            var result = _processExecutor.Start(
                _path,
                "dotnet",
                "test --no-build --no-restore",
                new Dictionary<string, string>
                {
                    {"ActiveMutation", activeMutationId.ToString() }
                },
                timeoutMS ?? 0);

            return new TestRunResult
            {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output
            };
        }

        public void Dispose()
        {
        }
    }
}