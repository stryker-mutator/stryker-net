using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class OpenReportEnabledInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveNoHelpText()
        {
            var target = new OpenReportEnabledInput();
            target.HelpText.ShouldBe(@" | default: 'False'");
        }

        [TestMethod]
        public void ShouldSetToTrue()
        {
            var target = new OpenReportEnabledInput { SuppliedInput = true };
            target.Validate().ShouldBeTrue();
        }

        [TestMethod]
        public void ShouldSetToFalse()
        {
            var target = new OpenReportEnabledInput { SuppliedInput = false };
            target.Validate().ShouldBeFalse();
        }
    }
}
