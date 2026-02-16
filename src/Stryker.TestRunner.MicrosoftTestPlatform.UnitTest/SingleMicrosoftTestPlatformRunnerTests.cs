using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class SingleMicrosoftTestPlatformRunnerTests
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

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up any temporary coverage files created during tests
        for (int id = 1; id <= 20; id++)
        {
            var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{id}.txt");
            try
            {
                if (File.Exists(coverageFilePath))
                {
                    File.Delete(coverageFilePath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [TestMethod]
    public void Constructor_ShouldCreateRunner()
    {
        // Act
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Assert
        runner.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldReturnFalse_WhenAssemblyNotFound()
    {
        // Arrange
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.DiscoverTestsAsync("/nonexistent/assembly.dll");

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task InitialTestAsync_ShouldReturnTestRunResult()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent/assembly.dll" });

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.InitialTestAsync(project.Object);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task TestMultipleMutantsAsync_ShouldReturnTestRunResult_WithNoAssemblies()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(1);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task TestMultipleMutantsAsync_ShouldUseCorrectMutantId_WhenSingleMutant()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(42);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        mutant.Verify(x => x.Id, Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task TestMultipleMutantsAsync_ShouldUseNoMutationId_WhenMultipleMutants()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant1 = new Mock<IMutant>();
        mutant1.Setup(x => x.Id).Returns(1);
        var mutant2 = new Mock<IMutant>();
        mutant2.Setup(x => x.Id).Returns(2);
        var mutants = new List<IMutant> { mutant1.Object, mutant2.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task Dispose_ShouldCleanUpResources()
    {
        // Arrange
        var testableRunner = new TestableRunner(
            123,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Verify mutant file was created
        testableRunner.MutantFilePath.ShouldNotBeNull();
        var mutantFilePath = testableRunner.MutantFilePath;

        // Create the mutant file manually to test deletion
        File.WriteAllText(mutantFilePath, "-1");
        File.Exists(mutantFilePath).ShouldBeTrue("Mutant file should exist before disposal");

        // Act
        testableRunner.Dispose();

        // Assert
        testableRunner.DisposedFlagWasSet.ShouldBeTrue("_disposed flag should be set to true");
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should execute once on first call");
        File.Exists(mutantFilePath).ShouldBeFalse("Mutant file should be deleted after disposal");

        // Act - Second disposal (verify idempotency via _disposed flag check)
        testableRunner.Dispose();

        // Assert
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should only execute once due to _disposed flag check preventing re-execution");
    }

    [TestMethod]
    public void SetCoverageMode_ShouldEnableCoverageMode()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            1,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        runner.SetCoverageMode(true);

        // Assert
        runner.IsCoverageModeEnabled.ShouldBeTrue();
    }

    [TestMethod]
    public void SetCoverageMode_ShouldDisableCoverageMode()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            2,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);

        // Act
        runner.SetCoverageMode(false);

        // Assert
        runner.IsCoverageModeEnabled.ShouldBeFalse();
    }

    [TestMethod]
    public void SetCoverageMode_ShouldBeIdempotent_WhenCalledWithTrue()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            3,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);

        // Act - Call with same value
        runner.SetCoverageMode(true);

        // Assert - Should still be enabled and not throw any exceptions
        runner.IsCoverageModeEnabled.ShouldBeTrue();
    }

    [TestMethod]
    public void SetCoverageMode_ShouldBeIdempotent_WhenCalledWithFalse()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            4,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Default is false, so calling it again should be idempotent
        runner.IsCoverageModeEnabled.ShouldBeFalse();

        // Act - Call with same value as default
        runner.SetCoverageMode(false);

        // Assert - Should still be disabled and not throw any exceptions
        runner.IsCoverageModeEnabled.ShouldBeFalse();
    }

    [TestMethod]
    public void SetCoverageMode_ShouldDeleteCoverageFile_WhenEnabling()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            5,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Create a coverage file
        File.WriteAllText(runner.CoverageFilePath, "1,2,3;4,5,6");
        File.Exists(runner.CoverageFilePath).ShouldBeTrue();

        // Act
        runner.SetCoverageMode(true);

        // Assert
        File.Exists(runner.CoverageFilePath).ShouldBeFalse("Coverage file should be deleted when enabling coverage mode");
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileDoesNotExist()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            6,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Ensure file doesn't exist
        if (File.Exists(runner.CoverageFilePath))
        {
            File.Delete(runner.CoverageFilePath);
        }

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileIsEmpty()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            7,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileContainsOnlyWhitespace()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            8,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "   \n\t  ");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldParseCoveredMutantsOnly()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            9,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldParseBothCoveredAndStaticMutants()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            10,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3;4,5,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleEmptyCoveredSection()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            11,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, ";4,5,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleEmptyStaticSection()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            12,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3;");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleDataWithSpaces()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            13,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, " 1 , 2 , 3 ; 4 , 5 , 6 ");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldSkipInvalidNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            14,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,invalid,3;4,bad,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 3 });
        staticMutants.ShouldBe(new[] { 4, 6 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleMixedValidAndInvalidData()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            15,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,,3,notanumber,5;,,7,xyz,9");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 3, 5 });
        staticMutants.ShouldBe(new[] { 7, 9 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldReturnEmptyLists_OnFileReadException()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            16,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Create a locked file by opening it exclusively
        using var fileStream = new FileStream(runner.CoverageFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var writer = new StreamWriter(fileStream);
        writer.Write("1,2,3;4,5,6");
        writer.Flush();

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleSingleMutantId()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            17,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "42;");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 42 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleLargeNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            18,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "2147483647;2147483646");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 2147483647 });
        staticMutants.ShouldBe(new[] { 2147483646 });
    }

    [TestMethod]
    public void ReadCoverageData_ShouldHandleNegativeNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            19,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "-1,2,-3;4,-5");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { -1, 2, -3 });
        staticMutants.ShouldBe(new[] { 4, -5 });
    }

    private class TestableRunner : SingleMicrosoftTestPlatformRunner
    {
        private int _disposeLogicExecutedCount;

        public TestableRunner(
            int id,
            Dictionary<string, List<TestNode>> testsByAssembly,
            Dictionary<string, MtpTestDescription> testDescriptions,
            TestSet testSet,
            object discoveryLock,
            ILogger logger)
            : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger)
        {
        }

        public bool DisposedFlagWasSet { get; private set; }
        public int DisposeLogicExecutedCount => _disposeLogicExecutedCount;
        public string MutantFilePath => Path.Combine(Path.GetTempPath(), $"stryker-mutant-123.txt");

        public override void Dispose(bool disposing)
        {
            var disposedField = typeof(SingleMicrosoftTestPlatformRunner).GetField("_disposed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var wasDisposedBefore = (bool)disposedField!.GetValue(this)!;

            base.Dispose(disposing);

            var wasDisposedAfter = (bool)disposedField!.GetValue(this)!;

            if (!wasDisposedBefore && wasDisposedAfter)
            {
                _disposeLogicExecutedCount++;
                DisposedFlagWasSet = true;
            }
        }
    }

    private class TestableRunnerForCoverage : SingleMicrosoftTestPlatformRunner
    {
        private readonly int _id;

        public TestableRunnerForCoverage(
            int id,
            Dictionary<string, List<TestNode>> testsByAssembly,
            Dictionary<string, MtpTestDescription> testDescriptions,
            TestSet testSet,
            object discoveryLock,
            ILogger logger)
            : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger)
        {
            _id = id;
        }

        public string CoverageFilePath => Path.Combine(Path.GetTempPath(), $"stryker-coverage-{_id}.txt");

        public bool IsCoverageModeEnabled
        {
            get
            {
                var field = typeof(SingleMicrosoftTestPlatformRunner).GetField("_coverageMode",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)field!.GetValue(this)!;
            }
        }
    }
}

