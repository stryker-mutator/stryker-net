using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class ConcurrentTestRunnersOptionsTests
    {
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
        [InlineData(1, 1, "Stryker is running in single threaded mode due to max concurrent testrunners being set to 1.")]
        [InlineData(2, 2, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.")]
        [InlineData(4, 4, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.")]
        [InlineData(8, 8, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.")]
        [InlineData(16, 16, "Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.")]
        public void WhenGivenValueIsPassedAsMaxConcurrentTestRunnersParam_ExpectedValueShouldBeSet_ExpectedMessageShouldBeLogged(int given, int expected, string logMessage)
        {
            var mockLogger = new MockLogger();

            var options = new StrykerOptions(logger: mockLogger, maxConcurrentTestRunners: given);
            options.ConcurrentTestrunners.ShouldBe(expected);

            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);
            if (!(logMessage is null) && expected > safeProcessorCount || expected == 1)
            {
                string formattedMessage = string.Format(logMessage, given, expected);
                mockLogger.LogHitCount.ShouldBe(1);
                mockLogger.LogMessages.ShouldHaveSingleItem(formattedMessage);
            }
        }
    }
}
