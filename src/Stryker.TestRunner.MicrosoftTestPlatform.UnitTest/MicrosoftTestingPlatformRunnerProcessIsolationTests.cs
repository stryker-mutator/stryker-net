using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MicrosoftTestingPlatformRunnerProcessIsolationTests
{
    [TestMethod, Timeout(2000)]
    public async Task DisableMixMutants_RestartsServerAroundEveryMutant()
    {
        var options = CreateOptions(OptimizationModes.DisableMixMutants);
        using var runner = new IsolationTrackingRunner(0, options.Object);
        var project = CreateProject();

        await runner.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(1)], null);
        await runner.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(2)], null);

        runner.Events.ShouldBe(["reset", "run:1", "reset", "reset", "run:2", "reset"]);
        runner.ReadActiveMutantId().ShouldBe(-1);
    }

    [TestMethod, Timeout(2000)]
    public async Task DefaultMode_ReusesServerBetweenMutants()
    {
        var options = CreateOptions(OptimizationModes.CoverageBasedTest);
        using var runner = new IsolationTrackingRunner(0, options.Object);
        var project = CreateProject();

        await runner.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(1)], null);
        await runner.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(2)], null);

        runner.Events.ShouldBe(["run:1", "run:2"]);
    }

    [TestMethod, Timeout(2000)]
    public async Task DisableMixMutants_IsolatesConcurrentRunnerSessions()
    {
        var options = CreateOptions(OptimizationModes.DisableMixMutants, concurrency: 2);
        var runners = new ConcurrentBag<IsolationTrackingRunner>();
        var bothRunsStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseRuns = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var startedRuns = 0;
        var runnerFactory = new Mock<ISingleRunnerFactory>();
        runnerFactory.Setup(factory => factory.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>(),
                It.IsAny<IStrykerOptions>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger, IStrykerOptions>(
                (id, _, _, _, _, _, runnerOptions) =>
                {
                    var runner = new IsolationTrackingRunner(id, runnerOptions, async () =>
                    {
                        if (Interlocked.Increment(ref startedRuns) == 2)
                        {
                            bothRunsStarted.SetResult();
                        }

                        await releaseRuns.Task.ConfigureAwait(false);
                    });
                    runners.Add(runner);
                    return runner;
                });

        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);
        var project = CreateProject();

        var firstRun = pool.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(1)], null);
        var secondRun = pool.TestMultipleMutantsAsync(project.Object, null, [CreateMutant(2)], null);
        await bothRunsStarted.Task;
        releaseRuns.SetResult();
        await Task.WhenAll(firstRun, secondRun);

        var usedRunners = runners.Where(runner => runner.Events.Any(@event => @event.StartsWith("run:", StringComparison.Ordinal))).ToList();
        usedRunners.Count.ShouldBe(2);
        usedRunners.SelectMany(runner => runner.Events.Where(@event => @event.StartsWith("run:", StringComparison.Ordinal)))
            .OrderBy(@event => @event)
            .ShouldBe(["run:1", "run:2"]);
        usedRunners.ShouldAllBe(runner =>
            runner.Events.Count == 3
            && runner.Events[0] == "reset"
            && runner.Events[2] == "reset");
    }

    private static Mock<IStrykerOptions> CreateOptions(OptimizationModes optimizationMode, int concurrency = 1)
    {
        var options = new Mock<IStrykerOptions>();
        options.Setup(option => option.Concurrency).Returns(concurrency);
        options.Setup(option => option.OptimizationMode).Returns(optimizationMode);
        return options;
    }

    private static Mock<IProjectAndTests> CreateProject()
    {
        var project = new Mock<IProjectAndTests>();
        project.Setup(value => value.GetTestAssemblies()).Returns(["test.dll"]);
        return project;
    }

    private static IMutant CreateMutant(int id)
    {
        var mutant = new Mock<IMutant>();
        mutant.Setup(value => value.Id).Returns(id);
        return mutant.Object;
    }

    private sealed class IsolationTrackingRunner : MicrosoftTestingPlatformRunner
    {
        private readonly Func<Task>? _runStarted;

        public IsolationTrackingRunner(int id, IStrykerOptions options, Func<Task>? runStarted = null)
            : base(
                id,
                new Dictionary<string, List<TestNode>>(),
                new Dictionary<string, MtpTestDescription>(),
                new TestSet(),
                new object(),
                NullLogger.Instance,
                options)
        {
            _runStarted = runStarted;
        }

        public List<string> Events { get; } = [];

        public int ReadActiveMutantId() => BitConverter.ToInt32(File.ReadAllBytes(MutantFilePath), 0);

        public override Task ResetServerAsync()
        {
            Events.Add("reset");
            return Task.CompletedTask;
        }

        internal override async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
            string assembly,
            ITimeoutValueCalculator? timeoutCalc,
            IReadOnlyList<IMutant>? mutants = null,
            Func<TestNode, bool>? testUidFilter = null)
        {
            Events.Add($"run:{ReadActiveMutantId()}");
            if (_runStarted is not null)
            {
                await _runStarted().ConfigureAwait(false);
            }

            return (
                new TestRunResult(
                    Array.Empty<IFrameworkTestDescription>(),
                    TestIdentifierList.NoTest(),
                    TestIdentifierList.NoTest(),
                    TestIdentifierList.NoTest(),
                    string.Empty,
                    [],
                    TimeSpan.Zero),
                false,
                null);
        }
    }
}
