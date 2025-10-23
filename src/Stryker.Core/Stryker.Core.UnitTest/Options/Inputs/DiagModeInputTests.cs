using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class DiagModeInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new DiagModeInput();
        target.HelpText.ShouldBe("""
                                 Stryker enters diagnostic mode. Useful when encountering issues.
                                 Setting this flag makes Stryker increase the debug level and log more information to help troubleshooting. | default: 'False'
                                 """);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(true, true)]
    [DataRow(null, false)]
    public void ShouldValidate(bool? input, bool expected)
    {
        var target = new DiagModeInput { SuppliedInput = input };

        var result = target.Validate();

        result.ShouldBe(expected);
    }
}
