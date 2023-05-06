using System.Linq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ProjectVersionInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ProjectVersionInput();
        target.HelpText.ShouldBe(@"Project version used in dashboard reporter and baseline feature. | default: ''");
    }

    [Fact]
    public void ProjectVersion_UsesSuppliedInput_IfDashboardReporterEnabled()
    {
        var suppliedInput = "test";
        var input = new ProjectVersionInput { SuppliedInput = suppliedInput };

        var result = input.Validate(reporters: new[] { Reporter.Dashboard }, withBaseline: false);
        result.ShouldBe(suppliedInput);
    }

    [Fact]
    public void ProjectVersion_UsesSuppliedInput_IfBaselineEnabled()
    {
        var suppliedInput = "test";
        var input = new ProjectVersionInput { SuppliedInput = suppliedInput };

        var result = input.Validate(reporters: Enumerable.Empty<Reporter>(), withBaseline: true);
        result.ShouldBe(suppliedInput);
    }

    [Fact]
    public void ProjectVersion_ShouldBeDefault_IfBaselineAndDashboardDisabled()
    {
        var suppliedInput = "test";
        var input = new ProjectVersionInput { SuppliedInput = suppliedInput };

        var result = input.Validate(reporters: Enumerable.Empty<Reporter>(), withBaseline: false);
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ProjectVersion_ShouldBeDefault_IfDashboardEnabledAndSuppliedInputNull()
    {
        var input = new ProjectVersionInput();

        var result = input.Validate(reporters: new[] { Reporter.Dashboard }, withBaseline: false);
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ProjectVersion_CannotBeEmpty_WhenBaselineEnabled()
    {
        var input = new ProjectVersionInput();

        var exception = Should.Throw<InputException>(() => {
            input.Validate(reporters: Enumerable.Empty<Reporter>(), withBaseline: true);
        });

        exception.Message.ShouldBe("Project version cannot be empty when baseline is enabled");
    }
}
