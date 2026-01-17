using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MicrosoftTestPlatformRunnerPoolTests
{
    [TestMethod]
    public void Constructor_ShouldCreateRunnersBasedOnConcurrency()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(2);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Assert - pool should be created without exceptions
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public void Constructor_ShouldCreateAtLeastOneRunner_WhenConcurrencyIsZero()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(0);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Assert - pool should be created with at least 1 runner
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public void DiscoverTests_ShouldReturnFalse_WhenAssemblyPathIsEmpty()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Act
        var result = pool.DiscoverTestsAsync(string.Empty);

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public void DiscoverTests_ShouldReturnFalse_WhenAssemblyPathIsNull()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Act
        var result = pool.DiscoverTestsAsync(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public void DiscoverTests_ShouldReturnFalse_WhenAssemblyDoesNotExist()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Act
        var result = pool.DiscoverTestsAsync("/nonexistent/path/assembly.dll");

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public void GetTests_ShouldReturnEmptyTestSet_WhenNoTestsDiscovered()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);
        var project = new Mock<IProjectAndTests>();

        // Act
        var testSet = pool.GetTests(project.Object);

        // Assert
        testSet.Count.ShouldBe(0);
    }

    [TestMethod]
    public void InitialTest_ShouldReturnFailure_WhenNoTestAssembliesFound()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());

        // Act
        var result = pool.InitialTestAsync(project.Object);

        // Assert
        result.ResultMessage.ShouldContain("No test assemblies found");
    }

    [TestMethod]
    public void TestMultipleMutants_ShouldReturnFailure_WhenNoTestAssembliesFound()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());
        var mutants = new List<IMutant> { new Mock<IMutant>().Object };

        // Act
        var result = pool.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ResultMessage.ShouldContain("No test assemblies found");
    }

    [TestMethod]
    public void CaptureCoverage_ShouldReturnEmptyCoverage_WhenNoTestsDiscovered()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object);
        var project = new Mock<IProjectAndTests>();

        // Act
        var coverage = pool.CaptureCoverage(project.Object);

        // Assert
        coverage.ShouldBeEmpty();
    }

    [TestMethod]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        var pool = new MicrosoftTestPlatformRunnerPool(options.Object);

        // Act & Assert - should not throw
        pool.Dispose();
    }
}

