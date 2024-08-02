using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Baseline;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class AzureFileStorageSasInputTests : TestBase
    {
        private const string ValidSasInput = "se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sv=2018-11-09&sr=s&sig=fzEyru3OpOpzkTpLfFjuI6TEhShY/dsad%3D";

        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new AzureFileStorageSasInput();
            target.HelpText.ShouldBe(@"A Shared Access Signature for Azure File Storage is required when Azure File Storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview | default: ''");
        }

        [TestMethod]
        public void ShouldReturnDefault_WhenBaselineIsDisabled()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = ValidSasInput };

            var result = target.Validate(BaselineProvider.AzureFileStorage, false);

            result.ShouldBe(string.Empty);
        }

        [TestMethod]
        public void ShouldReturnDefault_WhenProviderIsNotAzureFileStorage()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = ValidSasInput };

            var result = target.Validate(BaselineProvider.Dashboard, true);

            result.ShouldBe(string.Empty);
        }

        [TestMethod]
        public void Should_Accept_Valid_SAS()
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = ValidSasInput };

            var validatedSas = target.Validate(BaselineProvider.AzureFileStorage, true);

            validatedSas.ShouldBe("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sv=2018-11-09&sr=s&sig=fzEyru3OpOpzkTpLfFjuI6TEhShY/dsad%3D");
        }

        [TestMethod]
        [DataRow("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sr=s&sig=4324234")]
        [DataRow("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sr=s&sv=4324234")]
        [DataRow("se=2022-08-27T09%3A26%3A07Z&sp=rwdl&spr=https&sv=2018-11-09&sr=s&")]
        public void Should_Throw_Exception_When_Missing_Important_Keys(string input)
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = input };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage, true));

            exception.Message.ShouldBe("The azure file storage shared access signature is not in the correct format");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void Should_Throw_Exception_When_AzureSAS_nullOrWhitespace(string input)
        {
            var target = new AzureFileStorageSasInput { SuppliedInput = input };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage, true));

            exception.Message.ShouldBe("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
        }
    }
}
