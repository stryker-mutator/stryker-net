using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BuildPropertiesInputShould : TestBase
{
    [TestMethod]
    public void HaveHelpText()
    {
        var target = new BuildPropertiesInput();
        target.HelpText.ShouldBe(@"Allows to specify build properties that will be passed to MSBuild when building the project. Use 'key=value' format for each property | default: []");
    }

    [TestMethod]
    public void HaveEmptyDefault()
    {
        var target = new BuildPropertiesInput { SuppliedInput = [] };

        var result = target.Validate();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ReturnProperty()
    {
        var target = new BuildPropertiesInput { SuppliedInput = ["DESIGNTIME=false"]};

        var result = target.Validate();

        result.ShouldHaveSingleItem().Key.ShouldBe("DESIGNTIME");
    }

    [TestMethod]
    public void ReturnMultipleProperties()
    {
        var target = new BuildPropertiesInput { SuppliedInput = ["DESIGNTIME=false", "custom=42"]};

        var result = target.Validate();

        result.Count.ShouldBe(2);
    }

    [TestMethod]
    public void HandleSpacesInValue()
    {
        var target = new BuildPropertiesInput { SuppliedInput = ["DESIGNTIME=\"fa lse\"", "custom=42"]};

        var result = target.Validate();
        result.ShouldContainKey("DESIGNTIME");
        result["DESIGNTIME"].ShouldBe("fa lse");
    }
}
