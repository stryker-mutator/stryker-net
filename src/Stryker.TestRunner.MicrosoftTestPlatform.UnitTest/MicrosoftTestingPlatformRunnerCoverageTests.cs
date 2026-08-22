using System.IO.MemoryMappedFiles;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MicrosoftTestingPlatformRunnerCoverageTests
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

    private MicrosoftTestingPlatformRunner CreateRunner(int runnerId) =>
        new(runnerId,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

    [TestMethod]
    public async Task SetCoverageMode_ShouldEnableCoverageMode()
    {
        var runnerId = 600;
        string? coverageFilePath = null;

        try
        {
            using var runner = CreateRunner(runnerId);

            // Create a test assembly to trigger server creation
            var testAssembly = typeof(MicrosoftTestingPlatformRunnerCoverageTests).Assembly.Location;
            await runner.DiscoverTestsAsync(testAssembly);

            // Create an existing coverage file for the assembly that should be deleted
            coverageFilePath = runner.GetCoverageFilePath(testAssembly);
            await File.WriteAllTextAsync(coverageFilePath, "1,2,3");
            File.Exists(coverageFilePath).ShouldBeTrue("Setup: coverage file should exist before test");

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
            if (coverageFilePath is not null && File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldDisableCoverageMode()
    {
        var runnerId = 601;
        string? coverageFilePath = null;

        try
        {
            using var runner = CreateRunner(runnerId);

            var testAssembly = typeof(MicrosoftTestingPlatformRunnerCoverageTests).Assembly.Location;

            // Enable coverage mode first
            runner.SetCoverageMode(true);
            await runner.DiscoverTestsAsync(testAssembly);

            // Create a coverage file for the assembly
            coverageFilePath = runner.GetCoverageFilePath(testAssembly);
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
            if (coverageFilePath is not null && File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldNoOp_WhenModeIsAlreadySet()
    {
        var runnerId = 602;
        string? coverageFilePath = null;

        try
        {
            using var runner = CreateRunner(runnerId);

            var testAssembly = typeof(MicrosoftTestingPlatformRunnerCoverageTests).Assembly.Location;
            await runner.DiscoverTestsAsync(testAssembly);
            coverageFilePath = runner.GetCoverageFilePath(testAssembly);

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
            if (coverageFilePath is not null && File.Exists(coverageFilePath))
            {
                File.Delete(coverageFilePath);
            }
        }
    }

    [TestMethod]
    public async Task SetCoverageMode_ShouldRestartServers_WhenTogglingBetweenModes()
    {
        var runnerId = 603;

        using var runner = CreateRunner(runnerId);

        var testAssembly = typeof(MicrosoftTestingPlatformRunnerCoverageTests).Assembly.Location;

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
    public void GetCoverageFilePath_ShouldBeStablePerAssembly_AndUniqueAcrossAssemblies()
    {
        using var runner = CreateRunner(508);
        using var otherRunner = CreateRunner(509);
        using var sameIdRunner = CreateRunner(508);

        var pathA = runner.GetCoverageFilePath("/some/dir/Tests.dll");
        var pathASecondCall = runner.GetCoverageFilePath("/some/dir/Tests.dll");
        var pathB = runner.GetCoverageFilePath("/some/dir/OtherTests.dll");
        // Same file name in a different directory must still get its own coverage file
        var pathC = runner.GetCoverageFilePath("/another/dir/Tests.dll");
        var pathOtherRunner = otherRunner.GetCoverageFilePath("/some/dir/Tests.dll");
        var pathSameIdRunner = sameIdRunner.GetCoverageFilePath("/some/dir/Tests.dll");

        pathASecondCall.ShouldBe(pathA, "path should be stable for the same assembly");
        pathB.ShouldNotBe(pathA, "different assemblies should get different coverage files");
        pathC.ShouldNotBe(pathA, "same assembly file name in another directory should get its own coverage file");
        pathOtherRunner.ShouldNotBe(pathA, "different runners should get different coverage files");
        pathSameIdRunner.ShouldNotBe(pathA, "the per-instance nonce should separate runner instances even when they share an id");

        // The base name embeds the process id and a per-instance nonce so a run does not pick up
        // files written by an earlier (possibly crashed) run or a concurrent Stryker process
        Path.GetFileName(pathA).ShouldStartWith($"stryker-coverage-{Environment.ProcessId}-");

        // Long assembly names are truncated (the hash keeps the name unique), so the file name
        // stays well clear of path-length limits
        var longName = new string('a', 200) + ".dll";
        var longPath = runner.GetCoverageFilePath($"/some/dir/{longName}");
        var longPathOtherDir = runner.GetCoverageFilePath($"/another/dir/{longName}");
        Path.GetFileName(longPath).Length.ShouldBeLessThan(90);
        longPathOtherDir.ShouldNotBe(longPath, "truncated names must still be distinct via the path hash");
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        using var runner = CreateRunner(500);

        // Assign a coverage file for an assembly but never create the file
        runner.GetCoverageFilePath("Tests.dll");

        var result = runner.ReadCoverageData();

        result.CoveredMutants.ShouldBeEmpty();
        result.StaticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmpty_WhenFileIsEmpty()
    {
        using var runner = CreateRunner(501);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, string.Empty);

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
        using var runner = CreateRunner(502);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, "   \n\t  ");

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
        using var runner = CreateRunner(503);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3");

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
        using var runner = CreateRunner(504);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3;10,20");

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
        using var runner = CreateRunner(505);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, "42");

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
        using var runner = CreateRunner(506);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, ";5,6,7");

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
        using var runner = CreateRunner(507);
        var coverageFilePath = runner.GetCoverageFilePath("Tests.dll");

        try
        {
            File.WriteAllText(coverageFilePath, "1,2,3;");

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
    public void ReadCoverageData_ShouldUnionCoverageAcrossAssemblies()
    {
        // Regression test: every test assembly's host writes its own coverage file (the injected
        // MutantControl overwrites its file on process exit), and the runner must union them.
        // With a single shared file, the final flush replaced the others, so only one assembly's
        // coverage survived a multi-assembly run.
        using var runner = CreateRunner(510);
        var firstFilePath = runner.GetCoverageFilePath("FirstTests.dll");
        var secondFilePath = runner.GetCoverageFilePath("SecondTests.dll");
        var thirdFilePath = runner.GetCoverageFilePath("ThirdTests.dll");

        try
        {
            File.WriteAllText(firstFilePath, "1,2,3;10");
            File.WriteAllText(secondFilePath, "3,4;10,20");
            // The third assembly's host never wrote coverage (e.g. it crashed); it must not
            // prevent the other files from being read.

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(4);
            result.CoveredMutants.ShouldContain(1);
            result.CoveredMutants.ShouldContain(2);
            result.CoveredMutants.ShouldContain(3);
            result.CoveredMutants.ShouldContain(4);

            result.StaticMutants.Count.ShouldBe(2);
            result.StaticMutants.ShouldContain(10);
            result.StaticMutants.ShouldContain(20);
        }
        finally
        {
            foreach (var path in new[] { firstFilePath, secondFilePath, thirdFilePath })
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }

    [TestMethod]
    public void ReadCoverageData_ShouldUnionCoverageAcrossMutatedAssembliesOfOneTestHost()
    {
        // Regression test for the case where a test host references several mutated assemblies: the
        // host loads one copy of the injected MutantControl per mutated assembly, and every copy
        // resolves the single STRYKER_COVERAGE_FILE the runner handed out. Each copy therefore writes
        // its own file, whose name starts with the name the runner handed out (minus the extension).
        using var runner = CreateRunner(511);
        var handedOutPath = runner.GetCoverageFilePath("Tests.dll");
        var withoutExtension = Path.Combine(
            Path.GetDirectoryName(handedOutPath)!,
            Path.GetFileNameWithoutExtension(handedOutPath));
        var firstMutatedAssemblyPath = $"{withoutExtension}-FirstMutatedAssembly.txt";
        var secondMutatedAssemblyPath = $"{withoutExtension}-SecondMutatedAssembly.txt";

        try
        {
            File.WriteAllText(firstMutatedAssemblyPath, "1,2;10");
            File.WriteAllText(secondMutatedAssemblyPath, "2,3;20");

            var result = runner.ReadCoverageData();

            result.CoveredMutants.Count.ShouldBe(3,
                "coverage of every mutated assembly in the host must be unioned");
            result.CoveredMutants.ShouldContain(1);
            result.CoveredMutants.ShouldContain(2);
            result.CoveredMutants.ShouldContain(3);

            result.StaticMutants.Count.ShouldBe(2);
            result.StaticMutants.ShouldContain(10);
            result.StaticMutants.ShouldContain(20);
        }
        finally
        {
            foreach (var path in new[] { firstMutatedAssemblyPath, secondMutatedAssemblyPath })
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }

    [TestMethod]
    public void DeleteCoverageFiles_ShouldDeleteFilesWrittenByEveryMutatedAssembly()
    {
        // Stale files from a previous run must not be read as this run's coverage
        using var runner = CreateRunner(512);
        var handedOutPath = runner.GetCoverageFilePath("Tests.dll");
        var withoutExtension = Path.Combine(
            Path.GetDirectoryName(handedOutPath)!,
            Path.GetFileNameWithoutExtension(handedOutPath));
        var mutatedAssemblyPath = $"{withoutExtension}-SomeMutatedAssembly.txt";

        try
        {
            File.WriteAllText(mutatedAssemblyPath, "1,2;10");

            runner.SetCoverageMode(true);

            File.Exists(mutatedAssemblyPath).ShouldBeFalse(
                "per-mutated-assembly coverage files must be deleted when entering coverage mode");
        }
        finally
        {
            if (File.Exists(mutatedAssemblyPath))
            {
                File.Delete(mutatedAssemblyPath);
            }
        }
    }

    [TestMethod]
    public async Task ResetServerAsync_ShouldDisposeAndClearAllServers()
    {
        using var runner = CreateRunner(0);

        // Populate _assemblyServers by discovering tests against the real test assembly
        var testAssembly = typeof(MicrosoftTestingPlatformRunnerCoverageTests).Assembly.Location;
        await runner.DiscoverTestsAsync(testAssembly);

        runner._assemblyServers.ShouldNotBeEmpty("servers should be populated after discovery");

        await runner.ResetServerAsync();

        runner._assemblyServers.ShouldBeEmpty("all servers should be disposed and removed after reset");
    }

    // --- Per-test coverage epoch relay tests ---
    //
    // These exercise the runner side of the handshake documented on MutantControl's epoch poller:
    // the runner writes a request epoch, the (here: simulated) test host writes back an ack epoch once
    // it has flushed, and the runner waits for that ack before reading the coverage file.

    [TestMethod]
    public void EpochRelay_WriteEpochRequest_DoesNotTouchAckHalf()
    {
        var runnerId = 700;
        var epochFilePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-roundtrip.txt");

        try
        {
            using var runner = new MicrosoftTestingPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            runner.InitializeEpochFile(epochFilePath);
            runner.WriteEpochRequest(epochFilePath, 5);

            var found = MicrosoftTestingPlatformRunner.TryReadEpochAck(epochFilePath, out var ack);

            found.ShouldBeTrue();
            ack.ShouldBe(0, "ack should still be 0; WriteEpochRequest only touches the request half");
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
            using var runner = new MicrosoftTestingPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            runner.InitializeEpochFile(epochFilePath);

            // Write request AND ack = 3 directly, simulating the test host having already caught up.
            using (var stream = new FileStream(epochFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(stream, null, 8,
                       System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true))
            using (var accessor = mmf.CreateViewAccessor(0, 8, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
            {
                accessor.Write(0, 3);
                accessor.Write(4, 3);
            }

            var acked = await MicrosoftTestingPlatformRunner.WaitForEpochAckAsync(epochFilePath, 3, TimeSpan.FromSeconds(5));

            acked.ShouldBeTrue();
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
            using var runner = new MicrosoftTestingPlatformRunner(
                runnerId, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

            runner.InitializeEpochFile(epochFilePath);
            runner.WriteEpochRequest(epochFilePath, 1);

            // Nothing ever writes ack=1, so this must time out quickly rather than hang.
            var acked = await MicrosoftTestingPlatformRunner.WaitForEpochAckAsync(epochFilePath, 1, TimeSpan.FromMilliseconds(100));

            acked.ShouldBeFalse();
        }
        finally
        {
            if (File.Exists(epochFilePath)) File.Delete(epochFilePath);
        }
    }

    // A test host loads one copy of the injected MutantControl per mutated assembly, and every copy
    // runs its own epoch relay against its own relay file, derived from the path the runner handed out.
    // The runner must therefore request a flush from every relay and wait for all of them: waiting on a
    // single ack lets it read the coverage file while another copy has not flushed yet, which attributes
    // that assembly's coverage to the next test.

    private static string WriteEpochRelayFile(string basePath, string mutatedAssemblyName, int request, int ack)
    {
        var relayPath = Path.Combine(
            Path.GetDirectoryName(basePath)!,
            $"{Path.GetFileNameWithoutExtension(basePath)}-{mutatedAssemblyName}{Path.GetExtension(basePath)}");

        using var stream = new FileStream(relayPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        stream.SetLength(8);
        using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true);
        using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.ReadWrite);
        accessor.Write(0, request);
        accessor.Write(4, ack);
        accessor.Flush();

        return relayPath;
    }

    private static int ReadEpochRequest(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
        using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.Read);
        return accessor.ReadInt32(0);
    }

    [TestMethod]
    public void EpochRelay_BroadcastRequest_ReachesEveryMutatedAssemblyRelay()
    {
        var runnerId = 710;
        var basePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-broadcast.txt");
        var firstRelay = WriteEpochRelayFile(basePath, "FirstMutatedAssembly", request: 0, ack: 0);
        var secondRelay = WriteEpochRelayFile(basePath, "SecondMutatedAssembly", request: 0, ack: 0);

        try
        {
            using var runner = CreateRunner(runnerId);
            runner.InitializeEpochFile(basePath);

            runner.BroadcastEpochRequest(basePath, 4);

            ReadEpochRequest(firstRelay).ShouldBe(4);
            ReadEpochRequest(secondRelay).ShouldBe(4);
        }
        finally
        {
            foreach (var path in new[] { basePath, firstRelay, secondRelay })
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }

    [TestMethod, Timeout(10000)]
    public async Task EpochRelay_WaitForAllAcks_IgnoresARelayThatAppearedAfterTheRequest()
    {
        // A copy of the injected MutantControl creates its relay when the assembly it belongs to first
        // runs mutated code, which can happen while the runner is already waiting. Such a relay never
        // received the request, so it can never acknowledge it: waiting for it would burn the whole
        // timeout and throw away the coverage of a test that was in fact fully flushed. Its own coverage
        // is flushed on the next epoch instead.
        var runnerId = 713;
        var basePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-latecomer.txt");
        var reachedRelay = WriteEpochRelayFile(basePath, "AssemblyPresentAtRequest", request: 0, ack: 0);
        string? lateRelay = null;

        try
        {
            using var runner = CreateRunner(runnerId);
            runner.InitializeEpochFile(basePath);

            var requestedRelays = runner.BroadcastEpochRequest(basePath, 1);

            // The copy that shows up too late to be asked, and the one that was asked answering
            lateRelay = WriteEpochRelayFile(basePath, "AssemblyLoadedWhileWaiting", request: 0, ack: 0);
            WriteEpochRelayFile(basePath, "AssemblyPresentAtRequest", request: 1, ack: 1);

            var acked = await runner.WaitForAllEpochAcksAsync(requestedRelays, 1, TimeSpan.FromSeconds(5));

            acked.ShouldBeTrue("every relay the request reached has acknowledged it");
        }
        finally
        {
            foreach (var path in new[] { basePath, reachedRelay, lateRelay })
            {
                if (path is not null && File.Exists(path)) File.Delete(path);
            }
        }
    }

    [TestMethod, Timeout(10000)]
    public async Task EpochRelay_WaitForAllAcks_WaitsUntilEveryRelayHasFlushed()
    {
        var runnerId = 711;
        var basePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-waitall.txt");
        var firstRelay = WriteEpochRelayFile(basePath, "FirstMutatedAssembly", request: 2, ack: 2);
        var secondRelay = WriteEpochRelayFile(basePath, "SecondMutatedAssembly", request: 2, ack: 1);

        try
        {
            using var runner = CreateRunner(runnerId);
            runner.InitializeEpochFile(basePath);

            var requestedRelays = runner.BroadcastEpochRequest(basePath, 2);
            requestedRelays.Count.ShouldBe(2, "both relays existed when the request went out");

            var partiallyAcked = await runner.WaitForAllEpochAcksAsync(requestedRelays, 2, TimeSpan.FromMilliseconds(200));
            partiallyAcked.ShouldBeFalse("one relay is still one epoch behind, so its coverage is not on disk yet");

            WriteEpochRelayFile(basePath, "SecondMutatedAssembly", request: 2, ack: 2);

            var fullyAcked = await runner.WaitForAllEpochAcksAsync(requestedRelays, 2, TimeSpan.FromSeconds(5));
            fullyAcked.ShouldBeTrue("every relay has now flushed the epoch");
        }
        finally
        {
            foreach (var path in new[] { basePath, firstRelay, secondRelay })
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }

    [TestMethod, Timeout(5000)]
    public async Task EpochRelay_WaitForAllAcks_ReturnsTrue_WhenNoMutatedAssemblyRegistered()
    {
        // A test that touches no mutated code leaves no relay behind; there is nothing to flush, so the
        // wait must return at once instead of burning the timeout on every such test.
        var runnerId = 712;
        var basePath = Path.Combine(Path.GetTempPath(), $"stryker-epoch-{runnerId}-norelay.txt");

        try
        {
            using var runner = CreateRunner(runnerId);
            runner.InitializeEpochFile(basePath);

            var requestedRelays = runner.BroadcastEpochRequest(basePath, 1);
            requestedRelays.ShouldBeEmpty("no mutated assembly ran, so no copy created a relay");

            var acked = await runner.WaitForAllEpochAcksAsync(requestedRelays, 1, TimeSpan.FromSeconds(2));

            acked.ShouldBeTrue();
        }
        finally
        {
            if (File.Exists(basePath)) File.Delete(basePath);
        }
    }

    [TestMethod, Timeout(5000)]
    public async Task RunSingleTestForCoverageInReusedProcessAsync_ReturnsDubious_WhenServerCannotStart()
    {
        using var runner = new MicrosoftTestingPlatformRunner(
            703, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetPerTestCoverageMode(true);
        var testNode = new TestNode("test-1", "Test1", "test", "discovered");

        var result = await runner.RunSingleTestForCoverageInReusedProcessAsync("/nonexistent/assembly.dll", testNode, "test-1");

        result.Confidence.ShouldBe(CoverageConfidence.Dubious);
        result.MutationsCovered.ShouldBeEmpty();
    }

    // --- Isolated ("perTestInIsolation") per-test coverage capture ---
    //
    // The happyflow can't be tested here because it requires a real test host to run the test and flush coverage
    // to the coverage file. The happyflow is tested in the integration test project, which runs real tests in a real test host.

    [TestMethod]
    public void PerTestFilePaths_ShouldBeUniquePerRun_LikeTheWholeRunCoveragePath()
    {
        // These paths are read, overwritten and deleted while a run is in flight, so they have to name
        // one run and no other. Two Stryker processes on one machine hand their runners the same small
        // ids, and a crashed run leaves files a later one would find: the whole-run coverage path already
        // carries the process id and a per-instance nonce for exactly that reason.
        using var runner = CreateRunner(720);
        using var sameIdRunner = CreateRunner(720);

        var coveragePath = runner.GetPerTestCoverageFilePath("/some/dir/Tests.dll");
        var epochPath = runner.GetPerTestEpochFilePath("/some/dir/Tests.dll");

        coveragePath.ShouldBe(runner.GetPerTestCoverageFilePath("/some/dir/Tests.dll"),
            "the path must stay the same for the same assembly, it is written and read across a whole capture");
        coveragePath.ShouldNotBe(runner.GetPerTestCoverageFilePath("/some/dir/OtherTests.dll"),
            "different assemblies must not share a file");
        coveragePath.ShouldNotBe(epochPath, "the coverage file and the epoch relay are different files");

        sameIdRunner.GetPerTestCoverageFilePath("/some/dir/Tests.dll").ShouldNotBe(coveragePath,
            "two runner instances sharing an id must not share a file");
        sameIdRunner.GetPerTestEpochFilePath("/some/dir/Tests.dll").ShouldNotBe(epochPath,
            "two runner instances sharing an id must not share a relay");

        // A concurrent Stryker process must not land on these paths
        Path.GetFileName(coveragePath).ShouldStartWith($"stryker-coverage-pt-{Environment.ProcessId}-");
        Path.GetFileName(epochPath).ShouldStartWith($"stryker-epoch-{Environment.ProcessId}-");
    }

    [TestMethod, Timeout(10000)]
    public async Task SetPerTestCoverageMode_ShouldDeleteThePerTestFiles_WhenLeavingTheMode()
    {
        // The runner remembers which assemblies it set per-test files up for, and that record is what
        // names the files to delete when it is disposed. Leaving the mode used to drop the record, so by
        // the time anything came to clean up there was nothing left to name: every run leaked a coverage
        // file and an epoch relay per mutated assembly into the temp directory.
        const string assembly = "/nonexistent/assembly.dll";

        using var runner = CreateRunner(714);
        runner.SetPerTestCoverageMode(true);

        // Registers the assembly the way a real capture does, then fails for want of a test host
        await runner.RunSingleTestForCoverageInReusedProcessAsync(
            assembly, new TestNode("test-1", "Test1", "test", "discovered"), "test-1");

        var coverageFilePath = runner.GetPerTestCoverageFilePath(assembly);
        var epochFilePath = runner.GetPerTestEpochFilePath(assembly);
        // What the copies of the injected MutantControl in the host would have written
        var writtenByTheHost = new[]
        {
            SuffixedPath(coverageFilePath, "FirstMutatedAssembly"),
            SuffixedPath(coverageFilePath, "SecondMutatedAssembly"),
            SuffixedPath(epochFilePath, "FirstMutatedAssembly"),
            SuffixedPath(epochFilePath, "SecondMutatedAssembly")
        };

        try
        {
            foreach (var path in writtenByTheHost)
            {
                File.WriteAllText(path, "1,2;10");
            }

            runner.SetPerTestCoverageMode(false);

            foreach (var path in writtenByTheHost)
            {
                File.Exists(path).ShouldBeFalse($"{Path.GetFileName(path)} should not outlive the capture");
            }
            File.Exists(epochFilePath).ShouldBeFalse("the epoch file the runner created should go too");
        }
        finally
        {
            foreach (var path in writtenByTheHost.Concat(new[] { coverageFilePath, epochFilePath }))
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }

    private static string SuffixedPath(string path, string suffix) =>
        Path.Combine(
            Path.GetDirectoryName(path)!,
            $"{Path.GetFileNameWithoutExtension(path)}-{suffix}{Path.GetExtension(path)}");

    [TestMethod, Timeout(5000)]
    public async Task RunSingleTestForCoverageInIsolatedProcessAsync_ReturnsDubious_WhenServerCannotStart()
    {
        using var runner = new MicrosoftTestingPlatformRunner(
            710, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetCoverageMode(true);
        var testNode = new TestNode("test-1", "Test1", "test", "discovered");

        var result = await runner.RunSingleTestForCoverageInIsolatedProcessAsync(
            "/nonexistent/assembly.dll", testNode, "test-1");

        // A capture that never ran must not be reported as Exact - that confidence is what lets the
        // pool drop mutants no test covered, so a silent empty-but-Exact result would kill real mutants.
        result.Confidence.ShouldBe(CoverageConfidence.Dubious);
        result.MutationsCovered.ShouldBeEmpty();
        result.TestId.ShouldBe("test-1", "the result must stay attributable to the test that was asked for");
    }

    [TestMethod, Timeout(5000)]
    public async Task RunSingleTestForCoverageInIsolatedProcessAsync_ShouldDiscardCoverageLeftByAnEarlierTest()
    {
        // Every isolated capture runs in a host started for it alone, so anything already on disk was
        // written for an earlier test. The coverage files are per mutated assembly, so a host that does
        // not load one of them never rewrites its file: reading it would credit this test with the
        // earlier test's coverage, and at Exact confidence, which is trusted enough to drop mutants no
        // test covers. Clearing them first is what the single shared file used to give for free, since
        // every host rewrote it whole.
        const string assembly = "/nonexistent/assembly.dll";

        using var runner = CreateRunner(713);
        runner.SetCoverageMode(true);

        var handedOutPath = runner.GetCoverageFilePath(assembly);
        var earlierTestFilePath = Path.Combine(
            Path.GetDirectoryName(handedOutPath)!,
            $"{Path.GetFileNameWithoutExtension(handedOutPath)}-AssemblyFromAnEarlierTest.txt");

        try
        {
            File.WriteAllText(earlierTestFilePath, "1,2,3;10");

            await runner.RunSingleTestForCoverageInIsolatedProcessAsync(
                assembly, new TestNode("test-1", "Test1", "test", "discovered"), "test-1");

            File.Exists(earlierTestFilePath).ShouldBeFalse(
                "coverage written for an earlier test must not survive into this one");
        }
        finally
        {
            if (File.Exists(earlierTestFilePath))
            {
                File.Delete(earlierTestFilePath);
            }
        }
    }

    [TestMethod, Timeout(10000)]
    public async Task RunSingleTestForCoverageInIsolatedProcessAsync_ShouldKeepFailingCleanly_WhenCalledRepeatedly()
    {
        // Each isolated capture starts from scratch, so a failure for one test must not poison the
        // next: no dead server may be reused, and every test gets its own attributable result.
        const string assembly = "/nonexistent/assembly.dll";

        using var runner = new MicrosoftTestingPlatformRunner(
            712, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetCoverageMode(true);

        var first = await runner.RunSingleTestForCoverageInIsolatedProcessAsync(
            assembly, new TestNode("test-1", "Test1", "test", "discovered"), "test-1");
        var second = await runner.RunSingleTestForCoverageInIsolatedProcessAsync(
            assembly, new TestNode("test-2", "Test2", "test", "discovered"), "test-2");

        first.Confidence.ShouldBe(CoverageConfidence.Dubious);
        second.Confidence.ShouldBe(CoverageConfidence.Dubious);
        first.TestId.ShouldBe("test-1");
        second.TestId.ShouldBe("test-2");
        runner._assemblyServers.ShouldNotContainKey(assembly);
    }

    [TestMethod, Timeout(10000)]
    public async Task RunSingleTestForCoverageInIsolatedProcessAsync_ShouldClaimAggregateCoverageFile_NotThePerTestOne()
    {
        // The isolated path reads the aggregate per-assembly coverage file (the one MutantControl
        // flushes on process exit), NOT the "perTest" epoch-relay file. Reading the wrong file would
        // silently report empty coverage for every test, so assert the aggregate path was registered.
        // Deliberately checked before calling GetCoverageFilePath, which would register it itself.
        const string assembly = "/nonexistent/coverage-path.dll";

        using var runner = new MicrosoftTestingPlatformRunner(
            713, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        await runner.RunSingleTestForCoverageInIsolatedProcessAsync(
            assembly, new TestNode("test-1", "Test1", "test", "discovered"), "test-1");

        var registeredPaths = runner._coverageFilePaths;

        registeredPaths.ShouldContainKey(assembly,
            "the isolated capture must resolve this assembly's aggregate coverage file");

        var coverageFilePath = registeredPaths[assembly];
        // The per-test relay file (stryker-coverage-pt-) belongs to the reused-process mode.
        Path.GetFileName(coverageFilePath).ShouldStartWith("stryker-coverage-");
        Path.GetFileName(coverageFilePath).ShouldNotStartWith("stryker-coverage-pt-");

        try
        {
            // The path the capture resolved must be the one ReadCoverageData later drains.
            await File.WriteAllTextAsync(coverageFilePath, "1,2;9");

            var (covered, statics) = runner.ReadCoverageData();

            covered.ShouldBe(new[] { 1, 2 }, ignoreOrder: true);
            statics.ShouldBe(new[] { 9 }, ignoreOrder: true);
        }
        finally
        {
            if (File.Exists(coverageFilePath)) File.Delete(coverageFilePath);
        }
    }

    [TestMethod]
    public void SetPerTestCoverageMode_ShouldResetPerAssemblyState_WhenToggled()
    {
        using var runner = new MicrosoftTestingPlatformRunner(
            704, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

        runner.SetPerTestCoverageMode(true);
        runner._perTestCoverageMode.ShouldBeTrue();

        runner.SetPerTestCoverageMode(false);
        runner._perTestCoverageMode.ShouldBeFalse();
    }
}
