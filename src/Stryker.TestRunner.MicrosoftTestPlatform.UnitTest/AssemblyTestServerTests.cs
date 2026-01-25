using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class AssemblyTestServerTests
{
    [TestMethod]
    public void Constructor_ShouldCreateServer()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        // Act
        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Assert
        server.ShouldNotBeNull();
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_ShouldReturnFalse_WhenAssemblyNotFound()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Act
        var result = await server.StartAsync();

        // Assert
        result.ShouldBeFalse();
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_ShouldNotStartTwice_WhenAlreadyInitialized()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Manually set initialized to true using reflection to test the guard clause
        var isInitializedField = typeof(AssemblyTestServer).GetField("_isInitialized",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        isInitializedField?.SetValue(server, true);

        // Act
        var result = await server.StartAsync();

        // Assert
        result.ShouldBeTrue();
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldThrow_WhenNotInitialized()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await server.DiscoverTestsAsync();
        });
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldThrow_WhenNotInitialized()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await server.RunTestsAsync(null);
        });
    }

    [TestMethod]
    public async Task StopAsync_ShouldHandleMultipleCalls()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        using var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Act & Assert - should not throw
        await server.StopAsync();
        await server.StopAsync();
    }

    [TestMethod]
    public void Dispose_ShouldCleanUpResources()
    {
        // Arrange
        var assembly = "/nonexistent/assembly.dll";
        var envVars = new Dictionary<string, string?>();
        var logger = NullLogger.Instance;

        var server = new AssemblyTestServer(assembly, envVars, logger, "test-runner-1");

        // Act & Assert - should not throw
        server.Dispose();
        server.Dispose(); // Second dispose should be safe
    }
}

