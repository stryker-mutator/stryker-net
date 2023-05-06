using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class OpenReportInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new OpenReportInput();
        target.HelpText.ShouldBe(@"If enabled stryker will attempt to open the Html report in your browser when the results are calculated. | default: 'Html' | allowed: Html, Dashboard");
    }

    [Fact]
    public void ShouldHaveDefaultHtml()
    {
        var target = new OpenReportInput { SuppliedInput = null };
        var result = target.Validate(true);

        result.ShouldBe(ReportType.Html);
    }

    [Fact]
    public void ShouldReturnReportType()
    {
        var target = new OpenReportInput { SuppliedInput = "Html" };
        var result = target.Validate(true);

        result.ShouldBe(ReportType.Html);
    }

    [Fact]
    public void ShouldValidateReportType()
    {
        var target = new OpenReportInput { SuppliedInput = "gibberish" };
        var ex = Should.Throw<InputException>(() => target.Validate(true));

        ex.Message.ShouldBe("The given report type (gibberish) is invalid. Valid options are: [Html, Dashboard]");
    }
}
