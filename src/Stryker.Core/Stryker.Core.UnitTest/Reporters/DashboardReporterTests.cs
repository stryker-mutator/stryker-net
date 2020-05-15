using Moq;
using Stryker.Core.Clients;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                CurrentBranchCanonicalName = "refs/heads/master"
            };

            var dashboardClientMock = new Mock<IDashboardClient>(MockBehavior.Loose);

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
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
        public void ShouldCallChalkRedWhenReportUrlIsNull()
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

            var chalkMock = new Mock<IChalk>(MockBehavior.Loose);

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(default(string)));

            var target = new DashboardReporter(options, chalkMock.Object, dashboardClientMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            chalkMock.Verify(x => x.Red("Uploading to stryker dashboard failed..."));
        }

        [Fact]
        public void ShouldCallChalkGreenWhenReportUrl()
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

            var chalkMock = new Mock<IChalk>(MockBehavior.Loose);

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("www.example.com"));

            var target = new DashboardReporter(options, chalkMock.Object, dashboardClientMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            chalkMock.Verify(x => x.Green("\nYour stryker report has been uploaded to: \n www.example.com \nYou can open it in your browser of choice."));
        }
    }
}
