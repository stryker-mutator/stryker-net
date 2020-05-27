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
        public void ShouldCallUploadBaselineAndHumanReadableWhenCompareToDashboardEnabled()
        {
            // Arrange
            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters)
            {
            };

            var dashboardClientMock = new Mock<IDashboardClient>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IBranchProvider>();
            branchProviderMock.Setup(x => x.GetCurrentBranchCanonicalName()).Returns("refs/heads/master");
            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, branchProvider: branchProviderMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "refs/heads/master"), Times.Once);
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"), Times.Once);
        }

        [Fact]
        public void ShouldCallUploadHumanReadableWhenCompareToDashboardEnabled()
        {
            // Arrange
            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters
               );

            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"));

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"), Times.Once);
        }

        [Fact]
        public void ShouldCallUploadBaselineWithFallbackVersion()
        {
            // Arrange
            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallbackVersion");

            var dashboardClientMock = new Mock<IDashboardClient>(MockBehavior.Loose);

            var branchProviderMock = new Mock<IBranchProvider>();

            branchProviderMock.Setup(x => x.GetCurrentBranchCanonicalName()).Returns(string.Empty);

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, branchProviderMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "fallbackVersion"), Times.Once);
        }
    }
}
