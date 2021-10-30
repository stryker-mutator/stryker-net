using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OutputHtmlReportNameInputTests : TestBase
    {
        private string DefaultName = "mutation-report.html";

        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new OutputHtmlReportNameInput();
            target.HelpText.ShouldBe(@"");
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfEmptyString()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = string.Empty };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldUseDefaultFilenameIfNull()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = null };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfWhitespace()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = "  " };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldAppendExtensionIfMissingExtension()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = "report" };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe("report.html");
        }

        [Fact]
        public void ShouldNotAppendExtensionIfExtensionExists()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = "report.html" };
            var result = target.Validate();

            Assert.True(EndsInHTML(result));
            result.ShouldBe("report.html");
        }

        [Fact]
        public void ShouldNotAllowInvalidFilenames()
        {
            var target = new OutputHtmlReportNameInput() { SuppliedInput = new string(Path.GetInvalidPathChars()) };
            Assert.Throws<InputException>(() => target.Validate());
        }

        private bool EndsInHTML(string filename)
        {
            return filename.Contains(".html");
        }

      
    }
}
