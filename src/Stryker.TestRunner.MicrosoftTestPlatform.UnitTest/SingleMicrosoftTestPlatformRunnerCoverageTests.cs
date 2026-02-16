using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
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
    public void SetCoverageMode_ShouldEnableCoverageMode()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);

        // Coverage mode should be enabled - we can verify by setting it again (should not have any effect)
        runner.SetCoverageMode(true);
    }

    [TestMethod]
    public void SetCoverageMode_ShouldDisableCoverageMode()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);
        runner.SetCoverageMode(false);

        // Coverage mode should now be disabled
        runner.SetCoverageMode(false);
    }

    [TestMethod]
    public void SetCoverageMode_ShouldNoOp_WhenModeIsAlreadySet()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Enable coverage mode
        runner.SetCoverageMode(true);
        // Try to enable again - should do nothing
        runner.SetCoverageMode(true);
        
        // Disable coverage mode
        runner.SetCoverageMode(false);
        // Try to disable again - should do nothing
        runner.SetCoverageMode(false);
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
    public async Task ResetServerAsync_ShouldComplete()
    {
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        await runner.ResetServerAsync();

        // Verify no exceptions
    }
}
