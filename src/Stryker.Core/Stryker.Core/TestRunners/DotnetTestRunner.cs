using Stryker.Core.Testing;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private string _path { get; set; }
        private IProcessExecutor _processExecutor { get; set; }
        private int? _activeMutation { get; set; }

        public DotnetTestRunner(string path, IProcessExecutor processProxy)
        {
            _path = path;
            _processExecutor = processProxy;
        }

        public DotnetTestRunner(string path) : this(path, new ProcessExecutor()) { }

        public TestRunResult RunAll()
        {
            var result = _processExecutor.Start(
                _path,
                "dotnet",
                "test --no-build --no-restore",
                new Collection<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("ActiveMutation", _activeMutation.ToString())
                });

            return new TestRunResult() {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output
            };
        }

        public void SetActiveMutation(int? id)
        {
            _activeMutation = id;
        }
    }
}