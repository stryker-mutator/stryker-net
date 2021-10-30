using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OutputHTMLReportNameInputTests : TestBase
    {
        private string DefaultName = "mutation-report.html";

        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new OutputHTMLReportNameInput();
            target.HelpText.ShouldBe(@"");
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfEmptyString()
        {
            var target = new OutputHTMLReportNameInput() { SuppliedInput = string.Empty };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfNull()
        {
            var target = new OutputHTMLReportNameInput() { SuppliedInput = null };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfWhitespace()
        {
            var target = new OutputHTMLReportNameInput() { SuppliedInput = "  " };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldAppendExtensionIfMissingExtension()
        {
            var target = new OutputHTMLReportNameInput() { SuppliedInput = "report" };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe("report.html");
        }

        [Fact]
        public void ShouldNotAppendExtensionIfExtensionExists()
        {
            var target = new OutputHTMLReportNameInput() { SuppliedInput = "report.html" };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe("report.html");
        }

        private bool EndsInHTML(string filename)
        {
            return filename.Contains(".html");
        }

      
    }
}
