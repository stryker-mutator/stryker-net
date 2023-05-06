using System.Linq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ReportersInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ReportersInput();
        target.HelpText.ShouldBe("Reporters inform about various stages in the mutation testrun. | default: ['Progress', 'Html'] | allowed: All, Progress, Dots, ClearText, ClearTextTree, Json, Html, Dashboard, Markdown, Baseline");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new ReportersInput { SuppliedInput = null };

        var result = target.Validate(false);

        result.Count().ShouldBe(2);
        result.ShouldContain(Reporter.Progress);
        result.ShouldContain(Reporter.Html);
    }

    [Fact]
    public void ShouldReturnReporter()
    {
        var target = new ReportersInput { SuppliedInput = new[] { "Html", } };

        var result = target.Validate(false);

        result.ShouldHaveSingleItem().ShouldBe(Reporter.Html);
    }

    [Fact]
    public void ShouldReturnReporters()
    {
        var target = new ReportersInput { SuppliedInput = new[] {
            Reporter.Html.ToString(),
            Reporter.Json.ToString(),
            Reporter.Progress.ToString(),
            Reporter.Baseline.ToString(),
            Reporter.ClearText.ToString(),
            Reporter.ClearTextTree.ToString(),
            Reporter.Dashboard.ToString(),
            Reporter.Dots.ToString(),
        } };

        var result = target.Validate(false);

        result.Count().ShouldBe(8);
        result.ShouldContain(Reporter.Html);
        result.ShouldContain(Reporter.Json);
        result.ShouldContain(Reporter.Progress);
        result.ShouldContain(Reporter.Baseline);
        result.ShouldContain(Reporter.ClearText);
        result.ShouldContain(Reporter.ClearTextTree);
        result.ShouldContain(Reporter.Dashboard);
        result.ShouldContain(Reporter.Dots);
    }

    [Fact]
    public void ShouldValidateReporters()
    {
        var target = new ReportersInput { SuppliedInput = new[] { "Gibberish", "Test" } };

        var ex = Should.Throw<InputException>(() => target.Validate(false));

        ex.Message.ShouldBe($"These reporter values are incorrect: Gibberish, Test.");
    }

    [Fact]
    public void ShouldEnableBaselineReporterWhenWithBaselineEnabled()
    {
        var target = new ReportersInput { SuppliedInput = null };

        var validatedReporters = target.Validate(withBaseline: true);

        validatedReporters.ShouldContain(Reporter.Baseline);
    }
}
