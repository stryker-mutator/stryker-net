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

    private SingleMicrosoftTestPlatformRunner CreateRunner(int runnerId) =>
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
            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
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

            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;

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

            var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
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
    public async Task ResetServerAsync_ShouldDisposeAndClearAllServers()
    {
        using var runner = CreateRunner(0);

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

    [TestMethod]
    public async Task StopAndRemoveServerAsync_ShouldRemoveServerFromDictionary()
    {
        using var runner = CreateRunner(610);

        var testAssembly = typeof(SingleMicrosoftTestPlatformRunnerCoverageTests).Assembly.Location;
        await runner.DiscoverTestsAsync(testAssembly);

        var serversField = typeof(SingleMicrosoftTestPlatformRunner)
            .GetField("_assemblyServers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        var serversBefore = (Dictionary<string, AssemblyTestServer>)serversField.GetValue(runner)!;
        serversBefore.ShouldNotBeEmpty("servers should exist after discovery");

        await runner.StopAndRemoveServerAsync(testAssembly);

        var serversAfter = (Dictionary<string, AssemblyTestServer>)serversField.GetValue(runner)!;
        serversAfter.ContainsKey(testAssembly).ShouldBeFalse("server should be removed after stop");
    }

    [TestMethod]
    public void CaptureCoverageTestByTest_ShouldReturnDubious_WhenHandlerThrows()
    {
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        options.Setup(x => x.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest);

        var testNode = new TestNode("test-1", "ThrowingTest", "test", "discovered");
        var testsByAssembly = new Dictionary<string, List<TestNode>>
        {
            ["assembly.dll"] = [testNode]
        };
        var testDescriptions = new Dictionary<string, MtpTestDescription>
        {
            ["test-1"] = new(testNode)
        };

        var runnerFactory = new Mock<ISingleRunnerFactory>();
        runnerFactory.Setup(x => x.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>(),
                It.IsAny<IStrykerOptions>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger, IStrykerOptions>(
                (id, tba, td, ts, dl, logger, opts) =>
                {
                    if (tba.Count == 0)
                    {
                        foreach (var kvp in testsByAssembly)
                            tba[kvp.Key] = kvp.Value;
                        foreach (var kvp in testDescriptions)
                            td[kvp.Key] = kvp.Value;
                    }
                    return new TestableRunner(id, tba, td, ts, dl,
                        () => { },
                        coverageHandler: (_, _, _, _) =>
                            throw new InvalidOperationException("Server startup failed"));
                });

        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new[] { "assembly.dll" });

        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        var coverage = pool.CaptureCoverage(project.Object).ToList();

        coverage.Count.ShouldBe(1);
        coverage[0].Confidence.ShouldBe(CoverageConfidence.Dubious);
        coverage[0].MutationsCovered.ShouldBeEmpty();
    }

    [TestMethod]
    public void CaptureCoverageTestByTest_ShouldReturnDubious_WhenCoverageIsEmpty()
    {
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        options.Setup(x => x.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest);

        var testNode = new TestNode("test-1", "NoCoverageTest", "test", "discovered");
        var testsByAssembly = new Dictionary<string, List<TestNode>>
        {
            ["assembly.dll"] = [testNode]
        };
        var testDescriptions = new Dictionary<string, MtpTestDescription>
        {
            ["test-1"] = new(testNode)
        };

        var runnerFactory = new Mock<ISingleRunnerFactory>();
        runnerFactory.Setup(x => x.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>(),
                It.IsAny<IStrykerOptions>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger, IStrykerOptions>(
                (id, tba, td, ts, dl, logger, opts) =>
                {
                    if (tba.Count == 0)
                    {
                        foreach (var kvp in testsByAssembly)
                            tba[kvp.Key] = kvp.Value;
                        foreach (var kvp in testDescriptions)
                            td[kvp.Key] = kvp.Value;
                    }
                    return new TestableRunner(id, tba, td, ts, dl,
                        () => { },
                        coverageHandler: (_, _, testId, _) =>
                            Task.FromResult<ICoverageRunResult>(
                                CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                                    Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>())));
                });

        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new[] { "assembly.dll" });

        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        var coverage = pool.CaptureCoverage(project.Object).ToList();

        coverage.Count.ShouldBe(1);
        coverage[0].Confidence.ShouldBe(CoverageConfidence.Dubious);
        coverage[0].MutationsCovered.ShouldBeEmpty();
    }
}
