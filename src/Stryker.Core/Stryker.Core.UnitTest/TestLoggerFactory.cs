using Microsoft.Extensions.Logging;
using Moq;

namespace Stryker.Core.UnitTest;

/// <summary>
/// Helper class to create mock loggers for unit tests
/// </summary>
public static class TestLoggerFactory
{
    /// <summary>
    /// Creates a mock logger for testing
    /// </summary>
    public static ILogger<T> CreateLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }

    /// <summary>
    /// Creates a mock logger with setup for verification
    /// </summary>
    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }
}
