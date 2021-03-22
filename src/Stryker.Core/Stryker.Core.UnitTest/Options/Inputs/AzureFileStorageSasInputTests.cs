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

            Should.Throw<StrykerInputException>(act).Message.ShouldBe("A Shared Access Signature is required when Azure File Storage is enabled!");
        }

        [Fact]
        public void Should_Normalize_SAS()
        {
            var validatedSas = new AzureFileStorageSasInput { SuppliedInput = "?sv=SAS" }.Validate(BaselineProvider.AzureFileStorage);

            validatedSas.ShouldBe("SAS");
        }
    }
}
