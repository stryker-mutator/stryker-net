using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ReportFileNameInputTests : TestBase
    {
        private const string DefaultName = "mutation-report";

        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new ReportFileNameInput();
            target.HelpText.ShouldBe(@$" | default: '{target.Default}'");
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfEmptyString()
        {
            var target = new ReportFileNameInput() { SuppliedInput = string.Empty };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldUseDefaultFilenameIfNull()
        {
            var target = new ReportFileNameInput() { SuppliedInput = null };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldDefaultToMutationReportFilenameIfWhitespace()
        {
            var target = new ReportFileNameInput() { SuppliedInput = "  " };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [Fact]
        public void ShouldNotAllowInvalidFilenames()
        {
            var target = new ReportFileNameInput() { SuppliedInput = new string(Path.GetInvalidFileNameChars()) };
            Should.Throw<InputException>(() => target.Validate());
        }

        [Fact]
        public void ShouldNotAllowExtensions()
        {
            var target = new ReportFileNameInput() { SuppliedInput = "mutationReport.html" };
            Should.Throw<InputException>(() => target.Validate());
        }
    }
}
