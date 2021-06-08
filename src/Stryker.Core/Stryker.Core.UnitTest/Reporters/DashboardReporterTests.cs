using Moq;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class DashboardReporterTests
    {
        [Fact]
        public void ShouldUploadHumanReadableWhenCompareToDashboardEnabled()
        {
            // Arrange
            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
               dashboardApiKey: "Access_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters
               );

            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"));
            var branchProviderMock = new Mock<IGitInfoProvider>();

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"), Times.Once);
        }
    }
}
