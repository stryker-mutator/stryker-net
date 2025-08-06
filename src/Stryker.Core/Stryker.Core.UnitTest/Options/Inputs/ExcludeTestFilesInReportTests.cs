using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class ExcludeTestFilesInReportTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new ExcludeTestFilesInReportInput();
        target.HelpText.ShouldBe("Exclude test files in the report. This may reduce the size of the report significantly. | default: 'False'");
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    public void ShouldTranslateInputToExpectedResult(bool? argValue, bool expected)
    {
        var validatedInput = new ExcludeTestFilesInReportInput { SuppliedInput = argValue }.Validate();

        validatedInput.ShouldBe(expected);
    }
}
