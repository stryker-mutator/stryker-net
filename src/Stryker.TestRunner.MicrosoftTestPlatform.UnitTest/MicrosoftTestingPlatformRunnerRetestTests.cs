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

/// <summary>
/// The fresh-host retest of survivors a reused host hid: the decision table, the coverage-mode gate
/// and the pool summary (PR #3897 review follow-up).
/// </summary>
[TestClass]
public class MicrosoftTestingPlatformRunnerRetestTests
{
    private static TestRunResult PassedRun() => new(true);

    private static TestRunResult FailingRun() => new(
        System.Linq.Enumerable.Empty<IFrameworkTestDescription>(),
        new TestIdentifierList("t1"),
        new TestIdentifierList("t1"),
        TestIdentifierList.NoTest(),
        null,
        [],
        TimeSpan.Zero);

    private static TestRunResult TimedOutRun() => new(
        System.Linq.Enumerable.Empty<IFrameworkTestDescription>(),
        new TestIdentifierList("t1"),
        TestIdentifierList.NoTest(),
        new TestIdentifierList("t1"),
        null,
        [],
        TimeSpan.Zero);

    [TestMethod]
    [DataRow(1, true, false, false)]
    [DataRow(1, true, true, false)]
    [DataRow(1, false, false, true)]
    [DataRow(1, false, true, false)]
    [DataRow(-1, false, false, false)]
    public void ShouldRetestHiddenSurvivor_RetestsOnlyANotReachedPassingRun(int activeMutantId, bool reached, bool failing, bool expected)
    {
        var result = failing ? FailingRun() : PassedRun();

        MicrosoftTestingPlatformRunner.ShouldRetestHiddenSurvivor(result, reached, activeMutantId).ShouldBe(expected);
    }

    [TestMethod]
    public void ShouldRetestHiddenSurvivor_DoesNotRetestATimedOutRun()
    {
        // Not every test passed, so the run's outcome is already conclusive; the caller gates the
        // retest on the run not timing out either.
        MicrosoftTestingPlatformRunner.ShouldRetestHiddenSurvivor(TimedOutRun(), reached: false, activeMutantId: 1).ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldRetestHiddenSurvivor_DoesNotRetestARunWithoutExecutedTests()
    {
        // No executed tests means the run learned nothing; a retest on a fresh host would only repeat it.
        var result = new TestRunResult(
            System.Linq.Enumerable.Empty<IFrameworkTestDescription>(),
            TestIdentifierList.NoTest(),
            TestIdentifierList.NoTest(),
            TestIdentifierList.NoTest(),
            null,
            [],
            TimeSpan.Zero);

        MicrosoftTestingPlatformRunner.ShouldRetestHiddenSurvivor(result, reached: false, activeMutantId: 1).ShouldBeFalse();
    }

    [TestMethod]
    [DataRow(OptimizationModes.None, false)]
    [DataRow(OptimizationModes.SkipUncoveredMutants, true)]
    [DataRow(OptimizationModes.CoverageBasedTest, true)]
    [DataRow(OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest, true)]
    public void RetestHiddenSurvivorsEnabled_FollowsCoverageData(OptimizationModes mode, bool expected)
    {
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(o => o.OptimizationMode).Returns(mode);
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance, options.Object);

        runner.RetestHiddenSurvivorsEnabled.ShouldBe(expected);
    }

    [TestMethod]
    public void RetestHiddenSurvivorsEnabled_IsTrueWhenOptionsAreUnknown()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        runner.RetestHiddenSurvivorsEnabled.ShouldBeTrue();
    }

    [TestMethod]
    public void RetestStatistics_StartAtZero()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        runner.RetestStatistics.Retested.ShouldBe(0);
        runner.RetestStatistics.Killed.ShouldBe(0);
    }

    [TestMethod]
    public void ReachedFile_IsSizedAndClearedBeforeTheFirstRun()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        // The runner keeps the file open, so observe it the way a test host does: shared access.
        using var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var bytes = new byte[2 * sizeof(int)];
        stream.ReadExactly(bytes);
        BitConverter.ToInt32(bytes, 0).ShouldBe(0, "no signal before any run");
    }

    [TestMethod]
    public void ReachedFile_ReadsAsNotReached_WhenEmpty()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ReachedFile_ReadsAsReached_WhenTheHostWroteTheFlag()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;

        // Write the signal the way the injected MutantControl does through its memory-mapped view.
        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(42));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeTrue();
    }

    [TestMethod]
    public void ReachedFile_ReadsAsNotReached_WhenTheFlagBelongsToAnotherMutant()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;

        // A reset that failed leaves the previous run's flag in place; the recorded id must
        // disown it, or the stale signal would suppress the fresh-host retest.
        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(41));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ReachedFile_ReadsAsNotReached_AfterAReset()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;

        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(42));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeTrue();
        InvokeResetReachedFile(runner);

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ResetReachedFile_ReportsSuccess_WhenTheFileClears()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        runner.ResetReachedFile().ShouldBeTrue();
    }

    [TestMethod]
    public void ResetReachedFile_ReportsFailure_WhenTheFileCannotBeReopened()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);

        // Drop the cached stream so the reset has to reopen the file, then hold it exclusively
        // so that reopen fails with a sharing violation.
        LoseTheReachedStream(runner);

        using (var exclusive = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            runner.ResetReachedFile().ShouldBeFalse();
        }

        // The failure is not sticky: once the exclusive handle is gone the reset works again.
        runner.ResetReachedFile().ShouldBeTrue();
    }

    private static void LoseTheReachedStream(MicrosoftTestingPlatformRunner runner)
    {
        var field = typeof(MicrosoftTestingPlatformRunner).GetField(
            "_reachedFileStream", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        using var cached = (FileStream)field.GetValue(runner)!;
        field.SetValue(runner, null);
    }

    [TestMethod]
    public void ReadReachedFile_FallsBackToADiskRead_WhenTheCachedStreamIsLost()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;
        LoseTheReachedStream(runner);

        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(42));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeTrue();
    }

    [TestMethod]
    public void ReadReachedFile_Fallback_DoesNotTrustAFlagForAnotherMutant()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;
        LoseTheReachedStream(runner);

        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(41));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ReadReachedFile_ReturnsFalse_WhenTheRelayFileIsMissing()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        LoseTheReachedStream(runner);

        File.Delete(runner.ReachedFilePath);

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ReadReachedFile_ReturnsFalse_WhenTheFileHoldsOnlyTheFlag()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;
        LoseTheReachedStream(runner);

        // A host that died after writing the flag but before the id must read as not reached, not throw
        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            stream.Write(BitConverter.GetBytes(1));
            stream.Flush();
        }

        runner.ReadReachedFile().ShouldBeFalse();
    }

    [TestMethod]
    public void ReadReachedFile_ReturnsFalse_WhenTheFileCannotBeOpened()
    {
        using var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        runner.ActiveMutantId = 42;
        LoseTheReachedStream(runner);

        using (var stream = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(1));
            stream.Write(BitConverter.GetBytes(42));
            stream.Flush();
        }

        using (var exclusive = new FileStream(runner.ReachedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            runner.ReadReachedFile().ShouldBeFalse();
        }

        // Back to normal once the foreign handle is gone: the failure must not be cached.
        runner.ReadReachedFile().ShouldBeTrue();
    }

    [TestMethod]
    public void Dispose_RemovesTheReachedFile()
    {
        var runner = new MicrosoftTestingPlatformRunner(0, [], [], new TestSet(), new object(), NullLogger.Instance);
        File.Exists(runner.ReachedFilePath).ShouldBeTrue();

        runner.Dispose();

        File.Exists(runner.ReachedFilePath).ShouldBeFalse();
    }

    [TestMethod]
    public void PoolDispose_SummarizesTheRetestsAcrossRunners()
    {
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(o => o.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest);
        options.SetupGet(o => o.Concurrency).Returns(1);
        var logger = new TestLogger();

        var pool = new MicrosoftTestPlatformRunnerPool(options.Object, logger);
        var runner = pool.Runners.Single();
        SetRunnerStatistics(runner, retested: 2, killed: 1);

        pool.Dispose();

        var message = string.Join(Environment.NewLine, logger.Logs);
        message.ShouldContain("2 mutant(s) retested on a fresh host because reused-host state hid them; 1 killed");
    }

    [TestMethod]
    public void PoolDispose_KeepsSilent_WhenNoMutantWasRetested()
    {
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(o => o.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest);
        options.SetupGet(o => o.Concurrency).Returns(1);
        var logger = new TestLogger();

        var pool = new MicrosoftTestPlatformRunnerPool(options.Object, logger);
        pool.Dispose();

        var message = string.Join(Environment.NewLine, logger.Logs);
        message.ShouldNotContain("retested on a fresh host");
    }

    private static void SetRunnerStatistics(MicrosoftTestingPlatformRunner runner, int retested, int killed)
    {
        var type = typeof(MicrosoftTestingPlatformRunner);
        type.GetField("_retestCount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(runner, retested);
        type.GetField("_retestedKilledCount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(runner, killed);
    }

    private static void InvokeResetReachedFile(MicrosoftTestingPlatformRunner runner)
    {
        typeof(MicrosoftTestingPlatformRunner)
            .GetMethod("ResetReachedFile", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(runner, null);
    }

    private sealed class TestLogger : ILogger
    {
        public System.Collections.Generic.List<string> Logs { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Logs.Add(formatter(state, exception));
    }
}
