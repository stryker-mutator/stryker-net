using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ConcurrencyInputTests
    {
        private Mock<ILogger<StrykerOptions>> _loggerMock = new Mock<ILogger<StrykerOptions>>();

        [Fact]
        public void WhenZeroIsPassedAsMaxConcurrentTestRunnersParam_StrykerInputExceptionShouldBeThrown()
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ConcurrencyInput { SuppliedInput = 0 }.Validate(_loggerMock.Object);
            });
            ex.Message.ShouldBe("Concurrency must be at least 1.");
        }

        [Theory]
        [InlineData(2, LogLevel.Warning)]
        [InlineData(8, LogLevel.Warning)]
        [InlineData(16, LogLevel.Warning)]
        [InlineData(128, LogLevel.Warning)]
        public void WhenGivenValueIsPassedAsMaxConcurrentTestRunnersParam_ExpectedValueShouldBeSet_ExpectedMessageShouldBeLogged(int concurrentTestRunners, LogLevel expectedLoglevel)
        {
            var validatedInput = new ConcurrencyInput { SuppliedInput = concurrentTestRunners }.Validate(_loggerMock.Object);

            validatedInput.ShouldBe(concurrentTestRunners);

            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);

            if (concurrentTestRunners > safeProcessorCount)
            {
                string formattedMessage = string.Format("Using a concurrency of {0} which is more than recommended {1} for normal system operation. This might have an impact on performance.", concurrentTestRunners, safeProcessorCount);

                _loggerMock.Verify(expectedLoglevel, formattedMessage, Times.Once);
            }
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void WhenGiven1ShouldPrintWarning()
        {
            var validatedInput = new ConcurrencyInput { SuppliedInput = 1 }.Validate(_loggerMock.Object);

            validatedInput.ShouldBe(1);

            _loggerMock.Verify(LogLevel.Warning, "Stryker is running in single threaded mode due to concurrency being set to 1.", Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public void WhenGivenNullShouldGetDefault()
        {
            var validatedInput = new ConcurrencyInput().Validate(_loggerMock.Object);

            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);

            validatedInput.ShouldBe(safeProcessorCount);

            _loggerMock.VerifyNoOtherCalls();
        }
    }
}
