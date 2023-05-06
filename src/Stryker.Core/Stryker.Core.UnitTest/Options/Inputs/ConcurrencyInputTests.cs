using System;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ConcurrencyInputTests : TestBase
{
    private Mock<ILogger<ConcurrencyInput>> _loggerMock = new Mock<ILogger<ConcurrencyInput>>();

    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ConcurrencyInput();
        target.HelpText.ShouldBe($@"By default Stryker tries to make the most of your CPU, by spawning as many parallel processes as you have CPU cores.
This setting allows you to override this default behavior.
Reasons you might want to lower this setting:

    - Your test runner starts a browser (another CPU-intensive process)
    - You're running on a shared server
    - You are running stryker in the background while doing other work | default: '{Math.Max(Environment.ProcessorCount / 2, 1)}'");
    }

    [Fact]
    public void WhenZeroIsPassedAsMaxConcurrentTestRunnersParam_StrykerInputExceptionShouldBeThrown()
    {
        var target = new ConcurrencyInput { SuppliedInput = 0 };

        var ex = Assert.Throws<InputException>(() => target.Validate(_loggerMock.Object));

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

        var formattedMessage = string.Format("Stryker will use a max of {0} parallel testsessions.", concurrentTestRunners);
        _loggerMock.Verify(LogLevel.Information, formattedMessage, Times.Once);

        if (concurrentTestRunners > safeProcessorCount)
        {
            formattedMessage = string.Format("Using a concurrency of {0} which is more than recommended {1} for normal system operation. This might have an impact on performance.", concurrentTestRunners, safeProcessorCount);
            _loggerMock.Verify(expectedLoglevel, formattedMessage, Times.Once);
        }
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void WhenGiven1ShouldPrintWarning()
    {
        var validatedInput = new ConcurrencyInput { SuppliedInput = 1 }.Validate(_loggerMock.Object);

        validatedInput.ShouldBe(1);

        _loggerMock.Verify(LogLevel.Information, "Stryker will use a max of 1 parallel testsessions.", Times.Once);
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
