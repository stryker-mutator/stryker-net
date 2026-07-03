using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class SingleMicrosoftTestPlatformRunnerCoverageTests
{
    private Dictionary<string, List<TestNode>> _testsByAssembly = null!;
    private Dictionary<string, MtpTestDescription> _testDescriptions = null!;
    private TestSet _testSet = null!;
    private object _discoveryLock = null!;

    [TestInitialize]
    public void Initialize()
    {
        _testsByAssembly = new Dictionary<string, List<TestNode>>();
        _testDescriptions = new Dictionary<string, MtpTestDescription>();
        _testSet = new TestSet();
        _discoveryLock = new object();
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldEnableCoverageMode()
    {
        var runnerId = 600;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");
        
        try
        {
            // Create an existing coverage file that should be deleted
            await File.WriteAllTextAsync(coverageFilePath, "1,2,3");
            File.Exists(coverageFilePath).ShouldBeTrue("Setup: coverage file should exist before test");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            // Create a test assembly to trigger server creation
            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
            await runner.DiscoverTestsAsync(testAssembly);

            // Enable coverage mode
            runner.SetCoverageMode(true);

            // The old coverage file should be deleted
            File.Exists(coverageFilePath).ShouldBeFalse("Coverage file should be deleted when enabling coverage mode");

            // Servers should be disposed and will be recreated on next use with coverage env var
            // Verify we can still discover tests (which recreates servers)
            var result = await runner.DiscoverTestsAsync(testAssembly);
            result.ShouldBeTrue("Server should be recreated successfully after enabling coverage mode");

            // Trying to enable again should be a no-op
            await File.WriteAllTextAsync(coverageFilePath, "test");
            runner.SetCoverageMode(true);
            File.Exists(coverageFilePath).ShouldBeTrue("Should not delete file when mode is already enabled");
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldDisableCoverageMode()
    {
        var runnerId = 601;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");
        
        try
        {
            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;

            // Enable coverage mode first
            runner.SetCoverageMode(true);
            await runner.DiscoverTestsAsync(testAssembly);
            
            // Create a coverage file
            await File.WriteAllTextAsync(coverageFilePath, "1,2,3");
            File.Exists(coverageFilePath).ShouldBeTrue("Setup: coverage file should exist");

            // Disable coverage mode
            runner.SetCoverageMode(false);

            // The coverage file should be deleted when changing modes (clean start)
            File.Exists(coverageFilePath).ShouldBeFalse("Coverage file should be deleted when disabling coverage mode");

            // Servers should be disposed and will be recreated without coverage env var
            var result = await runner.DiscoverTestsAsync(testAssembly);
            result.ShouldBeTrue("Server should be recreated successfully after disabling coverage mode");

            // Trying to disable again should be a no-op (no servers disposed, no file deletion)
            await File.WriteAllTextAsync(coverageFilePath, "test");
            runner.SetCoverageMode(false);
            File.Exists(coverageFilePath).ShouldBeTrue("Should not delete file when mode is already disabled");
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldNoOp_WhenModeIsAlreadySet()
    {
        var runnerId = 602;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");
        
        try
        {
            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
            await runner.DiscoverTestsAsync(testAssembly);

            // Enable coverage mode
            runner.SetCoverageMode(true);
            File.Exists(coverageFilePath).ShouldBeFalse("Coverage file should be deleted on first enable");

            // Create a coverage file to verify no-op doesn't delete it
            await File.WriteAllTextAsync(coverageFilePath, "test-data");
            
            // Try to enable again - should do nothing (no server disposal, no file deletion)
            runner.SetCoverageMode(true);
            File.Exists(coverageFilePath).ShouldBeTrue("Coverage file should NOT be deleted when mode already enabled");
            (await File.ReadAllTextAsync(coverageFilePath)).ShouldBe("test-data", "File content should be unchanged");

            // Verify servers are still functional (not disposed)
            var result = await runner.DiscoverTestsAsync(testAssembly);
            result.ShouldBeTrue("Servers should still be functional after no-op");
            
            // Disable coverage mode
            runner.SetCoverageMode(false);
            
            // Try to disable again - should do nothing (no server disposal)
            runner.SetCoverageMode(false);
            
            // Verify servers are still functional
            result = await runner.DiscoverTestsAsync(testAssembly);
            result.ShouldBeTrue("Servers should still be functional after no-op disable");
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldRestartServers_WhenTogglingBetweenModes()
    {
        var runnerId = 603;

        using var runner = new SingleMicrosoftTestPlatformRunner(
            runnerId,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;

        // Initial discovery without coverage
        var result1 = await runner.DiscoverTestsAsync(testAssembly);
        result1.ShouldBeTrue("Initial discovery should succeed");

        // Enable coverage - should restart servers
        runner.SetCoverageMode(true);
        var result2 = await runner.DiscoverTestsAsync(testAssembly);
        result2.ShouldBeTrue("Discovery after enabling coverage should succeed (server restarted)");

        // Disable coverage - should restart servers again
        runner.SetCoverageMode(false);
        var result3 = await runner.DiscoverTestsAsync(testAssembly);
        result3.ShouldBeTrue("Discovery after disabling coverage should succeed (server restarted)");
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            500,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        var result = runner.ReadCoverageData();

        result.CoveredMutants.ShouldBeEmpty();
        result.StaticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmpty_WhenFileIsEmpty()
    {
        var runnerId = 501;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, string.Empty);

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.ShouldBeEmpty();
            result.StaticMutants.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmpty_WhenFileContainsWhitespace()
    {
        var runnerId = 502;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, "   \n\t  ");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.ShouldBeEmpty();
            result.StaticMutants.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldParseCoveredMutants()
    {
        var runnerId = 503;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(3);
            result.CoveredMutants.ShouldContain(1);
            result.CoveredMutants.ShouldContain(2);
            result.CoveredMutants.ShouldContain(3);
            result.StaticMutants.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldParseCoveredAndStaticMutants()
    {
        var runnerId = 504;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3;10,20");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(3);
            result.CoveredMutants.ShouldContain(1);
            result.CoveredMutants.ShouldContain(2);
            result.CoveredMutants.ShouldContain(3);
            
            result.StaticMutants.Count.ShouldBe(2);
            result.StaticMutants.ShouldContain(10);
            result.StaticMutants.ShouldContain(20);
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleSingleMutant()
    {
        var runnerId = 505;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, "42");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(1);
            result.CoveredMutants.ShouldContain(42);
            result.StaticMutants.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmptyCovered_WhenOnlyStaticMutantsPresent()
    {
        var runnerId = 506;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, ";5,6,7");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.ShouldBeEmpty();
            result.StaticMutants.Count.ShouldBe(3);
            result.StaticMutants.ShouldContain(5);
            result.StaticMutants.ShouldContain(6);
            result.StaticMutants.ShouldContain(7);
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleTrailingSemicolon()
    {
        var runnerId = 507;
        var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{runnerId}.txt");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3;");

            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                NullLogger.Instance);

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(3);
            result.StaticMutants.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task ResetServerAsync_ShouldDisposeAndClearAllServers()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Populate _assemblyServers by discovering tests against the real test assembly
        var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
        await runner.DiscoverTestsAsync(testAssembly);

        var serversField = typeof(SingleMicrosoftTestPlatformRunner)
            .GetField("_assemblyServers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        var serversBefore = (Dictionary<string, AssemblyTestServer>)serversField.GetValue(runner)!;
        serversBefore.ShouldNotBeEmpty("servers should be populated after discovery");

        await runner.ResetServerAsync();

        var serversAfter = (Dictionary<string, AssemblyTestServer>)serversField.GetValue(runner)!;
        serversAfter.ShouldBeEmpty("all servers should be disposed and removed after reset");
    }

    // --- Per-test coverage epoch relay tests ---
    //
    // These exercise the runner side of the handshake documented on MutantControl's epoch poller:
    // the runner writes a request epoch, the (here: simulated) test host writes back an ack epoch once
    // it has flushed, and the runner waits for that ack before reading the coverage file.

    private static object InvokePrivate(SingleMicrosoftTestPlatformRunner runner, string method, params object[] args)
    {
        var m = typeof(SingleMicrosoftTestPlatformRunner).GetMethod(method,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        return m.Invoke(runner, args)!;
    }

    [TestMethod]
    public void EpochRelay_WriteEpochRequest_DoesNotTouchAckHalf()
    {
        var runnerId = 700;
        var epochFilePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-roundtrip.txt");

        try
        {
            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            InvokePrivate(runner, "InitializeEpochFile", epochFilePath);
            InvokePrivate(runner, "WriteEpochRequest", epochFilePath, 5);

            var args = new object[] { epochFilePath, 0 };
            var found = (bool)typeof(SingleMicrosoftTestPlatformRunner)
                .GetMethod("TryReadEpochAck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(runner, args)!;

            found.ShouldBeTrue();
            ((int)args[1]).ShouldBe(0, "ack should still be 0; WriteEpochRequest only touches the request half");
        }
        finally
        {
            if (File.Exists(epochFilePath)) File.Delete(epochFilePath);
        }
    }

    [TestMethod]
    public async Task EpochRelay_WaitForAck_ReturnsTrue_WhenAckAlreadyMatches()
    {
        var runnerId = 701;
        var epochFilePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-match.txt");

        try
        {
            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            InvokePrivate(runner, "InitializeEpochFile", epochFilePath);

            // Write request AND ack = 3 directly, simulating the test host having already caught up.
            using (var stream = new FileStream(epochFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(stream, null, 8,
                       System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true))
            using (var accessor = mmf.CreateViewAccessor(0, 8, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
            {
                accessor.Write(0, 3);
                accessor.Write(4, 3);
            }

            var method = typeof(SingleMicrosoftTestPlatformRunner).GetMethod("WaitForEpochAckAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var task = (Task<bool>)method.Invoke(runner, new object[] { epochFilePath, 3, TimeSpan.FromSeconds(5) })!;

            (await task).ShouldBeTrue();
        }
        finally
        {
            if (File.Exists(epochFilePath)) File.Delete(epochFilePath);
        }
    }

    [TestMethod, Timeout(2000)]
    public async Task EpochRelay_WaitForAck_TimesOut_WhenAckNeverArrives()
    {
        var runnerId = 702;
        var epochFilePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-timeout.txt");

        try
        {
            using var runner = new SingleMicrosoftTestPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            InvokePrivate(runner, "InitializeEpochFile", epochFilePath);
            InvokePrivate(runner, "WriteEpochRequest", epochFilePath, 1);

            var method = typeof(SingleMicrosoftTestPlatformRunner).GetMethod("WaitForEpochAckAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            // Nothing ever writes ack=1, so this must time out quickly rather than hang.
            var task = (Task<bool>)method.Invoke(runner, new object[] { epochFilePath, 1, TimeSpan.FromMilliseconds(100) })!;

            (await task).ShouldBeFalse();
        }
        finally
        {
            if (File.Exists(epochFilePath)) File.Delete(epochFilePath);
        }
    }

    [TestMethod, Timeout(5000)]
    public async Task RunSingleTestForCoverageInReusedProcessAsync_ReturnsDubious_WhenServerCannotStart()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            703, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetPerTestCoverageMode(true);
        var testNode = new TestNode("test-1", "Test1", "test", "discovered");

        var result = await runner.RunSingleTestForCoverageInReusedProcessAsync("/nonexistent/assembly.dll", testNode, "test-1");

        result.Confidence.ShouldBe(CoverageConfidence.Dubious);
        result.MutationsCovered.ShouldBeEmpty();
    }

    [TestMethod]
    public void SetPerTestCoverageMode_ShouldResetPerAssemblyState_WhenToggled()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            704, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetPerTestCoverageMode(true);
        var modeField = typeof(SingleMicrosoftTestPlatformRunner)
            .GetField("_perTestCoverageMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        ((bool)modeField.GetValue(runner)!).ShouldBeTrue();

        runner.SetPerTestCoverageMode(false);
        ((bool)modeField.GetValue(runner)!).ShouldBeFalse();
    }
}
