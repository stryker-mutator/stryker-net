using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.DataCollector;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Stryker.Core.InjectedHelpers.Coverage;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly string _projectFile;
        private readonly IProcessExecutor _processExecutor;
        private readonly ILogger _logger;
        private readonly IEnumerable<string> _testBinariesPaths;
        private CommunicationServer _server;
        private CommunicationChannel _client;
        private readonly object _lck = new object();
        private string _lastMessage;

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, IEnumerable<string> testBinariesPaths)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
            _flags = flags;
            _projectFile = FilePathUtils.NormalizePathSeparators(path);
            _processExecutor = processProxy;
            _testBinariesPaths = testBinariesPaths;

            _server = new CommunicationServer("CoverageCollector");
            _server.SetLogger((msg) => _logger.LogDebug(msg));
            _server.RaiseNewClientEvent += ConnectionEstablished;
            _server.Listen();
        }

        public IEnumerable<TestDescription> Tests => null;

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant, TestUpdateHandler update)
        {
            var envVars = mutant == null ? null : 
                new Dictionary<string, string>
            {
                { "ActiveMutation", mutant.Id.ToString() }
            };
            try
            {
                var result = LaunchTestProcess(timeoutMs, envVars);
                update?.Invoke(new[] {mutant}, result.RanTests, result.FailingTests, result.TimedOutTests);
                return result;
            }
            catch (OperationCanceledException)
            {
                var emptyList = TestListDescription.NoTest();
                if (mutant != null)
                {
                    mutant.ResultStatus = MutantStatus.Timeout;
                }
                return TestRunResult.TimedOut(emptyList,  emptyList, TestListDescription.EveryTest(), "time out");
            }
        }

        private TestRunResult LaunchTestProcess(int? timeoutMs, IDictionary<string, string> envVars)
        {
            var result = _processExecutor.Start(
                _projectFile,
                "dotnet",
                @"vstest " + string.Join(" ", _testBinariesPaths),
                envVars,
                timeoutMs ?? 0);

            return new TestRunResult(result.ExitCode == 0, result.Output);
        }

        private void ConnectionEstablished(object s, ConnectionEventArgs e)
        {
            lock (_lck)
            {
                _client = e.Client;
                _client.RaiseReceivedMessage += ReceivedMutant;
                Monitor.Pulse(_lck);
            }
        }

        private void ReceivedMutant(object sender, string args)
        {
            lock (_lck)
            {
                _lastMessage = args;
                Monitor.Pulse(_lck);
            }
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool cantUseUnloadAppDomain, bool cantUsePipe)
        {
            if (!_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants) &&
                !_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return new TestRunResult(true);
            }

            if (cantUseUnloadAppDomain)
            {
                _logger.LogWarning("Can't capture the test coverage as the target framework does not support 'AppDomain'. ");
                return new TestRunResult(true);
            }

            if (cantUsePipe)
            {
                _logger.LogWarning("Can't capture the test coverage as the target framework does not support pipes. ");
                return new TestRunResult(true);
            }

            var collector = new CoverageCollector();
            collector.SetLogger(message => _logger.LogTrace(message));
            var coverageEnvironment = new Dictionary<string, string>();
            coverageEnvironment["Coverage"]= $"pipe:{_server.PipeName}";

            var result = LaunchTestProcess(null, coverageEnvironment);

            if (!WaitOnLck(_lck, () => _lastMessage != null, 5000))
            {
                // Failed to retrieve coverage data for testCase
                return null;
            }

            var testedMutant = _lastMessage.Split(";")[0].Split(",").Select(int.Parse).ToList();
            foreach (var mutant in mutants)
            {
                if (testedMutant.Contains(mutant.Id))
                {
                    mutant.DeclareCoveringTest(TestDescription.AllTests());
                }
                else
                {
                    mutant.ResetCoverage();
                }
            }

            return result;

        }

        static bool WaitOnLck(object lck, Func<bool> predicate, int timeout)
        {
            var watch = new Stopwatch();
            watch.Start();
            lock (lck)
            {
                while (!predicate() && watch.ElapsedMilliseconds <= timeout)
                {
                    Monitor.Wait(lck, Math.Max(1, (int)(timeout - watch.ElapsedMilliseconds)));
                }
            }

            return predicate();
        }

        public int DiscoverNumberOfTests()
        {
            return -1;
        }

        public void Dispose()
        {
        }
    }
}
