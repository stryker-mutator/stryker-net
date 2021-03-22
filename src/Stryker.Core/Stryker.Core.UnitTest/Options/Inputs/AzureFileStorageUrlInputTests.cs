using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class AzureFileStorageUrlInputTests
    {
        [Fact]
        public void Should_Throw_Exception_When_Azure_Storage_url_and_SAS_null()
        {
            void act() => new AzureFileStorageUrlInput { SuppliedInput = null }.Validate(BaselineProvider.AzureFileStorage);

            Should.Throw<StrykerInputException>(act).Message.ShouldBe(@"The url pointing to your file storage is required when Azure File Storage is enabled!");
        }
    }
}
