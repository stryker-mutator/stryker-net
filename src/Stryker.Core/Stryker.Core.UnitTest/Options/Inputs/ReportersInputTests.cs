using System.Linq;
using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Stryker.Configuration.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class ReportersInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ReportersInput();
            target.HelpText.ShouldBe("Reporters inform about various stages in the mutation testrun. | default: ['Progress', 'Html'] | allowed: All, Progress, Dots, ClearText, ClearTextTree, Json, Html, Dashboard, RealTimeDashboard, Markdown, Baseline");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var target = new ReportersInput { SuppliedInput = null };

            var result = target.Validate(false);

            result.Count().ShouldBe(2);
            result.ShouldContain(Reporter.Progress);
            result.ShouldContain(Reporter.Html);
        }

        [TestMethod]
        public void ShouldReturnReporter()
        {
            var target = new ReportersInput { SuppliedInput = new[] { "Html", } };

            var result = target.Validate(false);

            result.ShouldHaveSingleItem().ShouldBe(Reporter.Html);
        }

        [TestMethod]
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

        [TestMethod]
        public void ShouldValidateReporters()
        {
            var target = new ReportersInput { SuppliedInput = new[] { "Gibberish", "Test" } };

            var ex = Should.Throw<InputException>(() => target.Validate(false));

            ex.Message.ShouldBe($"These reporter values are incorrect: Gibberish, Test.");
        }

        [TestMethod]
        public void ShouldEnableBaselineReporterWhenWithBaselineEnabled()
        {
            var target = new ReportersInput { SuppliedInput = null };

            var validatedReporters = target.Validate(withBaseline: true);

            validatedReporters.ShouldContain(Reporter.Baseline);
        }
    }
}
