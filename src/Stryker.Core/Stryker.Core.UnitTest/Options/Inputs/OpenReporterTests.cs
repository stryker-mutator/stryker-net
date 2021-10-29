using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OpenReporterTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new OpenReporterInput();
            target.HelpText.ShouldBe(@"The dashboard to open automatically | default: 'None' | allowed: None, HTMLReport");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new OpenReporterInput { SuppliedInput = null };
            var result = target.Validate();

            result.ShouldBe(ReportType.None);
        }

        [Fact]
        public void ShouldReturnReportType()
        {
            var target = new OpenReporterInput { SuppliedInput = "HTMLReport" };
            var result = target.Validate();

            result.ShouldBe(ReportType.HTMLReport);
        }

        [Fact]
        public void ShouldValidateReportType()
        {
            var target = new OpenReporterInput { SuppliedInput = "gibberish" };
            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe("The given report (gibberish) is invalid. Valid options are: [None, HTMLReport]");
        }
    }
}
