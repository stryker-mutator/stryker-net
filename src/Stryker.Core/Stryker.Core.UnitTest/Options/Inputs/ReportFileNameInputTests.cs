using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class ReportFileNameInputTests : TestBase
    {
        private const string DefaultName = "mutation-report";

        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ReportFileNameInput();
            target.HelpText.ShouldBe(@$" | default: '{target.Default}'");
        }

        [TestMethod]
        public void ShouldDefaultToMutationReportFilenameIfEmptyString()
        {
            var target = new ReportFileNameInput() { SuppliedInput = string.Empty };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [TestMethod]
        public void ShouldUseDefaultFilenameIfNull()
        {
            var target = new ReportFileNameInput() { SuppliedInput = null };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [TestMethod]
        public void ShouldDefaultToMutationReportFilenameIfWhitespace()
        {
            var target = new ReportFileNameInput() { SuppliedInput = "  " };
            var result = target.Validate();

            result.ShouldBe(DefaultName);
        }

        [TestMethod]
        public void ShouldNotAllowInvalidFilenames()
        {
            var target = new ReportFileNameInput() { SuppliedInput = new string(Path.GetInvalidFileNameChars()) };
            Should.Throw<InputException>(() => target.Validate());
        }

        [TestMethod]
        public void ShouldStripHtmlAndJsonFileExtensions()
        {
            var target = new ReportFileNameInput() { SuppliedInput = $"{DefaultName}.html" };
            target.Validate().ShouldBe(DefaultName);
            target = new ReportFileNameInput() { SuppliedInput = $"{DefaultName}.json" };
            target.Validate().ShouldBe(DefaultName);
        }

        [TestMethod]
        public void ShouldNotStripNoneHtmlAndJsonFileExtensions()
        {
            var input = $"{DefaultName}.project";
            var target = new ReportFileNameInput() { SuppliedInput = input };
            target.Validate().ShouldBe(input);
        }
    }
}
