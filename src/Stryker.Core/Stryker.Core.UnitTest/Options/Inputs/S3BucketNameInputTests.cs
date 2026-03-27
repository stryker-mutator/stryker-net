using Shouldly;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class S3BucketNameInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new S3BucketNameInput();
        target.HelpText.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenProviderNotS3()
    {
        var target = new S3BucketNameInput { SuppliedInput = null };

        var result = target.Validate(BaselineProvider.Dashboard, true);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenBaselineIsDisabled()
    {
        var target = new S3BucketNameInput { SuppliedInput = "my-bucket" };

        var result = target.Validate(BaselineProvider.S3, false);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnBucketName_WhenValid()
    {
        var target = new S3BucketNameInput { SuppliedInput = "my-stryker-bucket" };

        var result = target.Validate(BaselineProvider.S3, true);

        result.ShouldBe("my-stryker-bucket");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ShouldThrowException_WhenBucketNameMissing(string input)
    {
        var target = new S3BucketNameInput { SuppliedInput = input };

        var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.S3, true));

        exception.Message.ShouldBe("The S3 bucket name is required when S3 is used as the baseline provider.");
    }
}
