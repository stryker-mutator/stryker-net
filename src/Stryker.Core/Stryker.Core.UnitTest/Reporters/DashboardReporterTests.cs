using System.IO;
using System.Threading.Tasks;
using Moq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Clients;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class DashboardReporterTests : TestBase
    {
        [Fact]
        public void ShouldUploadHumanReadableWhenCompareToDashboardEnabled()
        {
            // Arrange
            var reporters = new[] { Reporter.Dashboard };

            var options = new StrykerOptions {
               DashboardApiKey = "Access_Token",
               ProjectName = "github.com/JohnDoe/project",
               ProjectVersion = "version/human/readable",
               Reporters = reporters
            };

            var mockProcess = new Mock<IProcessWrapper>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable"));
            var branchProviderMock = new Mock<IGitInfoProvider>();

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, processWrapper: mockProcess.Object);

            // Act
            target.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable"), Times.Once);
        }

        [Fact]
        public void ShouldOpenDashboardReportIfOptionIsProvided()
        {
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Dashboard,
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IProcessWrapper>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable"))
                .Returns(Task.FromResult("https://dashboard.com"));

            var reporter = new DashboardReporter(options, dashboardClientMock.Object, processWrapper: mockProcess.Object);
            var mutationTree = JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent();

            reporter.OnAllMutantsTested(mutationTree);

            // Check if browser open action is invoked
            mockProcess.Verify(m => m.StartUrl("https://dashboard.com"));
        }

        [Theory]
        [InlineData(ReportType.Html)]
        [InlineData(null)]
        public void ShouldNotOpenDashboardReportIfOptionIsProvided(ReportType? reportType)
        {
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                ReportTypeToOpen = reportType,
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IProcessWrapper>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable"))
                .Returns(Task.FromResult("https://dashboard.com"));

            var reporter = new DashboardReporter(options, dashboardClientMock.Object, processWrapper: mockProcess.Object);
            var mutationTree = JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent();

            reporter.OnAllMutantsTested(mutationTree);

            // Check if browser open action is invoked
            mockProcess.VerifyNoOtherCalls();
        }
    }
}
