using Shouldly;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class S3EndpointInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new S3EndpointInput();
        target.HelpText.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenProviderNotS3()
    {
        var target = new S3EndpointInput { SuppliedInput = "https://minio.example.com" };

        var result = target.Validate(BaselineProvider.Dashboard, true);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenBaselineIsDisabled()
    {
        var target = new S3EndpointInput { SuppliedInput = "https://minio.example.com" };

        var result = target.Validate(BaselineProvider.S3, false);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnEndpoint_WhenValid()
    {
        var target = new S3EndpointInput { SuppliedInput = "https://minio.example.com:9000" };

        var result = target.Validate(BaselineProvider.S3, true);

        result.ShouldBe("https://minio.example.com:9000");
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ShouldReturnDefault_WhenEndpointNotProvided(string input)
    {
        var target = new S3EndpointInput { SuppliedInput = input };

        var result = target.Validate(BaselineProvider.S3, true);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldThrowException_OnInvalidUri()
    {
        var target = new S3EndpointInput { SuppliedInput = "not-a-url" };

        var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.S3, true));

        exception.Message.ShouldBe("The S3 endpoint is not a valid Uri: not-a-url");
    }
}
