using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class AzureFileStorageSasInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new AzureFileStorageSasInput();
            target.HelpText.ShouldBe(@"A Shared Access Signature for Azure File Storage is required when Azure File Storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview | default: ''");
        }

        [Fact]
        public void Should_Normalize_SAS()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = "?sv=SAS" };

            var validatedSas = target.Validate(BaselineProvider.AzureFileStorage);

            validatedSas.ShouldBe("SAS");
        }

        [Fact]
        public void Should_Throw_Exception_When_AzureSAS_null()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = null };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage));

            exception.Message.ShouldBe("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
        }
    }
}
