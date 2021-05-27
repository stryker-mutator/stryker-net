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
        public void Should_Throw_Exception_When_AzureSAS_null()
        {
            void act() => new AzureFileStorageSasInput { SuppliedInput = null }.Validate(BaselineProvider.AzureFileStorage);

            Should.Throw<InputException>(act).Message.ShouldBe("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
        }

        [Fact]
        public void Should_Normalize_SAS()
        {
            var validatedSas = new AzureFileStorageSasInput { SuppliedInput = "?sv=SAS" }.Validate(BaselineProvider.AzureFileStorage);

            validatedSas.ShouldBe("SAS");
        }
    }
}
