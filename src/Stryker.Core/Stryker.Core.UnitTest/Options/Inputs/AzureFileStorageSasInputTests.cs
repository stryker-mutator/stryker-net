using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class AzureFileStorageSasInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new AzureFileStorageSasInput();
            target.HelpText.ShouldBe(@"A Shared Access Signature for Azure File Storage is required when Azure File Storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview | default: ''");
        }

        [Fact]
        public void Should_Accept_Valid_SAS()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = "se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sv=2018-11-09&sr=s&sig=fzEyru3OpOpzkTpLfFjuI6TEhShY/dsad%3D" };

            var validatedSas = target.Validate(BaselineProvider.AzureFileStorage);

            validatedSas.ShouldBe("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sv=2018-11-09&sr=s&sig=fzEyru3OpOpzkTpLfFjuI6TEhShY/dsad%3D");
        }

        [Theory]
        [InlineData("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sr=s&sig=4324234")]
        [InlineData("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sr=s&sv=4324234")]
        public void Should_Throw_Exception_When_Missing_Important_Keys(string input)
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = input };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage));

            exception.Message.ShouldBe("The azure file storage shared access signature is not in the correct format");
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
