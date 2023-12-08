using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class AzureFileStorageUrlInputTests : TestBase
    {
        private const string ValidUrlInput = "http://example.com:8042";

        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new AzureFileStorageUrlInput();
            target.HelpText.ShouldBe(@"The url for the Azure File Storage is only needed when the Azure baseline provider is selected.
The url should look something like this:
https://STORAGE_NAME.file.core.windows.net/FILE_SHARE_NAME
Note, the url might be different depending on where your file storage is hosted. | default: ''");
        }

        [Fact]
        public void ShouldReturnDefault_WhenProviderNotAzureFileStorage()
        {
            var target = new AzureFileStorageUrlInput { SuppliedInput = null };

            var result = target.Validate(BaselineProvider.Dashboard, true);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void ShouldReturnDefault_WhenBaselineIsDisabled()
        {
            var target = new AzureFileStorageUrlInput { SuppliedInput = null };

            var result = target.Validate(BaselineProvider.AzureFileStorage, false);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void ShouldAllowUri()
        {
            var target = new AzureFileStorageUrlInput { SuppliedInput = ValidUrlInput };

            var result = target.Validate(BaselineProvider.AzureFileStorage, true);

            result.ShouldBe("http://example.com:8042");
        }

        [Fact]
        public void ShouldThrowException_WhenAzureStorageUrlAndSASNull()
        {
            var target = new AzureFileStorageUrlInput { SuppliedInput = null };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage, true));

            exception.Message.ShouldBe(@"The Azure File Storage url is required when Azure File Storage is used for dashboard compare.");
        }

        [Fact]
        public void ShouldThrowException_OnInvalidUri()
        {
            var target = new AzureFileStorageUrlInput { SuppliedInput = "test" };

            var exception = Should.Throw<InputException>(() => target.Validate(BaselineProvider.AzureFileStorage, true));

            exception.Message.ShouldBe("The Azure File Storage url is not a valid Uri: test");
        }
    }
}
