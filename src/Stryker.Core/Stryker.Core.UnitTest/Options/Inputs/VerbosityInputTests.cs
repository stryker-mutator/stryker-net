using Serilog.Events;
using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class VerbosityInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new VerbosityInput();
            target.HelpText.ShouldBe(@"The verbosity (loglevel) for output to the console. | default: 'info' | allowed: error, warning, info, debug, trace");
        }

        [TestMethod]
        public void ShouldBeInformationWhenNull()
        {
            var input = new VerbosityInput { SuppliedInput = null };
            var validatedInput = input.Validate();

            validatedInput.ShouldBe(LogEventLevel.Information);
        }

        [TestMethod]
        [DataRow("error", LogEventLevel.Error)]
        [DataRow("warning", LogEventLevel.Warning)]
        [DataRow("info", LogEventLevel.Information)]
        [DataRow("debug", LogEventLevel.Debug)]
        [DataRow("trace", LogEventLevel.Verbose)]
        public void ShouldTranslateLogLevelToLogEventLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var validatedInput = new VerbosityInput { SuppliedInput = argValue }.Validate();

            validatedInput.ShouldBe(expectedLogLevel);
        }

        [TestMethod]
        [DataRow("incorrect")]
        [DataRow("")]
        public void ShouldThrowWhenInputCannotBeTranslated(string logLevel)
        {
            var ex = Should.Throw<InputException>(() =>
            {
                new VerbosityInput { SuppliedInput = logLevel }.Validate();
            });

            ex.Message.ShouldBe($"Incorrect verbosity ({logLevel}). The verbosity options are [Trace, Debug, Info, Warning, Error]");
        }
    }
}
