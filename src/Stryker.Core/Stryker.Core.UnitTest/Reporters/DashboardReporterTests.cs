using Microsoft.Extensions.Logging;
using Moq;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using System;
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
            var branchProviderMock = new Mock<IGitInfoProvider>();
            branchProviderMock.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");
            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, gitInfoProvider: branchProviderMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "dashboard-compare/refs/heads/master"), Times.Once);
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
            var branchProviderMock = new Mock<IGitInfoProvider>();

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, branchProviderMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<string>(), "version/human/readable"), Times.Once);
        }

        [Fact]
        public void LogsDebugWhenBaselineUploadedSuccesfull()
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
            var loggerMock = new Mock<ILogger<DashboardReporter>>(MockBehavior.Loose);
            var dashboardClientMock = new Mock<IDashboardClient>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IGitInfoProvider>();
            var chalkMock = new Mock<IChalk>();

            branchProviderMock.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");
            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), "dashboard-compare/refs/heads/master")).ReturnsAsync("http://www.example.com/baseline");
            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<string>(), "version/human/readable")).ReturnsAsync("http://www.example.com/humanreadable");
            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, gitInfoProvider: branchProviderMock.Object, loggerMock.Object, chalkMock.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());

            // Assert
            loggerMock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
            chalkMock.Verify(x => x.Green(It.Is<string>(s => s == "Your stryker report has been uploaded to: \n http://www.example.com/humanreadable \nYou can open it in your browser of choice.")));
        }
    }
}
