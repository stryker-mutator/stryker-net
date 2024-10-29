using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Stryker.Core.UnitTest;

public static class LoggerMockExtensions
{
    /// <summary>
    /// Verifies that the given message was logged with the given LogLevel one time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mock"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    public static void Verify<T>(this Mock<ILogger<T>> mock, LogLevel logLevel, string message)
    {
        mock.Verify(logLevel, message, Times.Once);
    }

    /// <summary>
    /// Verifies that the given message was logged with the given LogLevel given times.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mock"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    /// <param name="times"></param>
    public static void Verify<T>(this Mock<ILogger<T>> mock, LogLevel logLevel, string message, Func<Times> times)
    {
        mock.Verify(logLevel, message, times());
    }

    /// <summary>
    /// Verifies that the given message was logged with the given LogLevel given times.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mock"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    /// <param name="times"></param>
    public static void Verify<T>(this Mock<ILogger<T>> mock, LogLevel logLevel, string message, Times times)
    {
        mock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals(message, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), times
        );
    }
}
