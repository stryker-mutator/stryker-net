using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class ConcurrentTestRunnersOptionsTests
    {
        private Mock<ILogger<StrykerOptions>> _loggerMock = new Mock<ILogger<StrykerOptions>>();

        [Fact]
        public void WhenZeroIsPassedAsMaxConcurrentTestRunnersParam_StrykerInputExceptionShouldBeThrown()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerOptions(maxConcurrentTestRunners: 0);
            });
            ex.Details.ShouldBe("Amount of maximum concurrent testrunners must be greater than zero.");
        }

        [Theory]
        [InlineData(2, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.", LogLevel.Warning)]
        [InlineData(8, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.", LogLevel.Warning)]
        [InlineData(16, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.", LogLevel.Warning)]
        [InlineData(128, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.", LogLevel.Warning)]
        public void WhenGivenValueIsPassedAsMaxConcurrentTestRunnersParam_ExpectedValueShouldBeSet_ExpectedMessageShouldBeLogged(int concurrentTestRunners, string logMessage, LogLevel expectedLoglevel)
        {
            var options = new StrykerOptions(logger: _loggerMock.Object, maxConcurrentTestRunners: concurrentTestRunners);

            options.Concurrency.ShouldBe(concurrentTestRunners);

            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);

            if (concurrentTestRunners > safeProcessorCount)
            {
                string formattedMessage = string.Format(logMessage, concurrentTestRunners, safeProcessorCount);

                _loggerMock.Verify(expectedLoglevel, formattedMessage, Times.Once);
            }
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void WhenGiven1ShouldPrintWarning()
        {
            var options = new StrykerOptions(logger: _loggerMock.Object, maxConcurrentTestRunners: 1);

            options.ConcurrentTestrunners.ShouldBe(1);

            _loggerMock.Verify(LogLevel.Warning, "Stryker is running in single threaded mode due to max concurrent testrunners being set to 1.", Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public void WhenGivenNullShouldGetDefault()
        {
            var options = new StrykerOptions(logger: _loggerMock.Object, maxConcurrentTestRunners: null);

            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);

            options.ConcurrentTestrunners.ShouldBe(safeProcessorCount);

            _loggerMock.VerifyNoOtherCalls();
        }
    }
}
