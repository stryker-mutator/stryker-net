using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        InitialTestRun InitialTest(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner);
    }

    public class InitialTestProcess : IInitialTestProcess
    {
        private readonly ILogger _logger;

        public InitialTestProcess() => _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();

        public ITimeoutValueCalculator TimeoutValueCalculator { get; private set; }

        /// <summary>
        /// Executes the initial test run using the given testrunner
        /// </summary>
        /// <param name="project"></param>
        /// <param name="testRunner"></param>
        /// <param name="options">Stryker options</param>
        /// <returns>The duration of the initial test run</returns>
        public InitialTestRun InitialTest(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner)
        {
            // Setup a stopwatch to record the initial test duration
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var initTestRunResult = testRunner.InitialTest(project);
            // Stop stopwatch immediately after test run
            stopwatch.Stop();

            // timings
            _logger.LogDebug("Initial test run output: {ResultMessage}.", initTestRunResult.ResultMessage);

            TimeoutValueCalculator = new TimeoutValueCalculator(options.AdditionalTimeout,
                (int)stopwatch.ElapsedMilliseconds,
                (int)initTestRunResult.Duration.TotalMilliseconds);

            return new InitialTestRun(initTestRunResult, TimeoutValueCalculator);
        }
    }
}
