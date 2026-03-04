using Shouldly;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class AwsS3BucketNameInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new AwsS3BucketNameInput();
        target.HelpText.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenProviderNotAwsS3()
    {
        var target = new AwsS3BucketNameInput { SuppliedInput = null };

        var result = target.Validate(BaselineProvider.Dashboard, true);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenBaselineIsDisabled()
    {
        var target = new AwsS3BucketNameInput { SuppliedInput = "my-bucket" };

        var result = target.Validate(BaselineProvider.AWSS3, false);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnBucketName_WhenValid()
    {
        var target = new AwsS3BucketNameInput { SuppliedInput = "my-stryker-bucket" };

        var result = target.Validate(BaselineProvider.AWSS3, true);

        result.ShouldBe("my-stryker-bucket");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ShouldThrowException_WhenBucketNameMissing(string input)
    {
        var target = new AwsS3BucketNameInput { SuppliedInput = input };

        var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AWSS3, true));

        exception.Message.ShouldBe("The AWS S3 bucket name is required when AWSS3 is used as the baseline provider.");
    }
}
