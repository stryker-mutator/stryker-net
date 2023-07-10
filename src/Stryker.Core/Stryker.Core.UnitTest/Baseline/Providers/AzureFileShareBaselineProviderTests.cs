using Azure.Storage.Files.Shares;
using Moq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers
{
    public class AzureFileShareBaselineProviderTests : TestBase
    {
        [Fact]
        public void Load_WithValidSas_ReturnsReport()
        {
            // Arrange


            // Act
            var report = new AzureFileShareBaselineProvider(new StrykerOptions { AzureFileStorageSas = "" }, Mock.Of<ShareClient>());

            // Assert
        }
    }
}
