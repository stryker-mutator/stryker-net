using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OpenReportEnabledInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new OpenReportEnabledInput();
            target.HelpText.ShouldBe(@"When enabled the report will open automatically after stryker has generated the report. | default: 'False'");
        }

        [Fact]
        public void ShouldSetToTrue()
        {
            var target = new OpenReportEnabledInput { SuppliedInput = true };
            target.Validate().ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetToFalse()
        {
            var target = new OpenReportEnabledInput { SuppliedInput = false };
            target.Validate().ShouldBeFalse();
        }
    }
}
