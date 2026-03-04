using Shouldly;
using Stryker.Abstractions.Baseline;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class AwsS3RegionInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new AwsS3RegionInput();
        target.HelpText.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenProviderNotAwsS3()
    {
        var target = new AwsS3RegionInput { SuppliedInput = "us-east-1" };

        var result = target.Validate(BaselineProvider.Dashboard, true);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenBaselineIsDisabled()
    {
        var target = new AwsS3RegionInput { SuppliedInput = "us-east-1" };

        var result = target.Validate(BaselineProvider.AWSS3, false);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnRegion_WhenValid()
    {
        var target = new AwsS3RegionInput { SuppliedInput = "eu-west-1" };

        var result = target.Validate(BaselineProvider.AWSS3, true);

        result.ShouldBe("eu-west-1");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ShouldReturnDefault_WhenRegionNotProvided(string input)
    {
        var target = new AwsS3RegionInput { SuppliedInput = input };

        var result = target.Validate(BaselineProvider.AWSS3, true);

        result.ShouldBe(string.Empty);
    }
}
