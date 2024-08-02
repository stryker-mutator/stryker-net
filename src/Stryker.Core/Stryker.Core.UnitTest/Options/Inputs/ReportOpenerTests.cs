using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class ReportOpenerTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new OpenReportInput();
            target.HelpText.ShouldBe(@"When this option is passed, generated reports should open in the browser automatically once Stryker starts testing mutants, and will update the report till Stryker is done. Both html and dashboard reports can be opened automatically. | default: 'Html' | allowed: Html, Dashboard");
        }

        [TestMethod]
        public void ShouldHaveDefaultHtml()
        {
            var target = new OpenReportInput { SuppliedInput = null };
            var result = target.Validate(true);

            result.ShouldBe(ReportType.Html);
        }

        [TestMethod]
        public void ShouldReturnReportType()
        {
            var target = new OpenReportInput { SuppliedInput = "Html" };
            var result = target.Validate(true);

            result.ShouldBe(ReportType.Html);
        }

        [TestMethod]
        public void ShouldValidateReportType()
        {
            var target = new OpenReportInput { SuppliedInput = "gibberish" };
            var ex = Should.Throw<InputException>(() => target.Validate(true));

            ex.Message.ShouldBe("The given report type (gibberish) is invalid. Valid options are: [Html, Dashboard]");
        }
    }
}
