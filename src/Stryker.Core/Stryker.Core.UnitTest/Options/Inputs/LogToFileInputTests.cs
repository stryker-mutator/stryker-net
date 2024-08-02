using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class LogToFileInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new LogToFileInput();
            target.HelpText.ShouldBe(@"Makes the logger write to a file. Logging to file always uses loglevel trace. | default: 'False'");
        }

        [TestMethod]
        public void ShouldThrowIfTrueAndNoOutputPath()
        {
            var target = new LogToFileInput { SuppliedInput = true };

            var exception = Should.Throw<InputException>(() => target.Validate(null));

            exception.Message.ShouldBe("Output path must be set if log to file is enabled");
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(null, false)]
        public void ShouldValidate(bool? input, bool expected)
        {
            var target = new LogToFileInput { SuppliedInput = input };

            var result = target.Validate("TestPath");

            result.ShouldBe(expected);
        }
    }
}
