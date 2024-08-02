using System.Linq;
using System.Threading.Tasks;
using Moq;
using Stryker.Configuration.Clients;
using Stryker.Configuration;
using Stryker.Configuration.Options.Inputs;
using Stryker.Configuration.ProjectComponents.TestProjects;
using Stryker.Configuration.Reporters;
using Stryker.Configuration.Reporters.Json;
using Stryker.Configuration.Reporters.Json.SourceFiles;
using Stryker.Configuration.Reporters.WebBrowserOpener;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Reporters
{
    [TestClass]
    public class DashboardReporterTests : TestBase
    {
        [TestMethod]
        public void ShouldUploadHumanReadableWhenCompareToDashboardEnabled()
        {
            // Arrange
            var reporters = new[] { Reporter.Dashboard };

            var options = new StrykerOptions
            {
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };

            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable", false));

            var target = new DashboardReporter(options, dashboardClient: dashboardClientMock.Object, browser: mockProcess.Object);

            // Act
            target.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>());

            // Assert
            dashboardClientMock.Verify(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable", false), Times.Once);
        }

        [TestMethod]
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
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable", true))
                .Returns(Task.FromResult("https://dashboard.com"));

            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnMutantsCreated(mutationTree, It.IsAny<TestProjectsInfo>());

            // Check if browser open action is invoked
            mockProcess.Verify(m => m.Open("https://dashboard.com"));
        }

        [TestMethod]
        public void ShouldNotOpenDashboardWithRealTimeDashboardOptionButItShouldUploadTheInitialReport()
        {
            var reporters = new[] { Reporter.RealTimeDashboard };
            var options = new StrykerOptions
            {
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable", true))
                .Returns(Task.FromResult("https://dashboard.com"));

            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnMutantsCreated(mutationTree, It.IsAny<TestProjectsInfo>());

            mockProcess.VerifyNoOtherCalls();
            dashboardClientMock.Verify(d => d.PublishReport(It.IsAny<JsonReport>(), It.IsAny<string>(), true));
        }

        [TestMethod]
        public void ShouldNotDoAnythingIfNotOpeningTheDashboardAndIfNotRealTimeDashboardReporter()
        {
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();
            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnMutantsCreated(mutationTree, It.IsAny<TestProjectsInfo>());

            mockProcess.VerifyNoOtherCalls();
            dashboardClientMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [DataRow(ReportType.Html)]
        [DataRow(null)]
        public void ShouldNotOpenDashboardReportIfOptionIsProvided(ReportType? reportType)
        {
            var reporters = new[] { Reporter.Dashboard, Reporter.RealTimeDashboard };
            var options = new StrykerOptions
            {
                ReportTypeToOpen = reportType,
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();

            dashboardClientMock.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), "version/human/readable", false))
                .Returns(Task.FromResult("https://dashboard.com"));

            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnMutantsCreated(mutationTree, It.IsAny<TestProjectsInfo>());

            // Check if browser open action is invoked
            mockProcess.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShouldSendMutantBatchIfOpenDashboardOptionIsProvided()
        {
            // Arrange
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Dashboard,
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();
            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();
            var mutant = mutationTree.Mutants.First();

            // Act
            reporter.OnMutantTested(mutant);

            // Assert
            mockProcess.VerifyNoOtherCalls();
            dashboardClientMock.Verify(d => d.PublishMutantBatch(It.IsAny<JsonMutant>()));
        }

        [TestMethod]
        public void ShouldSendMutantBatchWithRealTimeDashboardOption()
        {
            // Arrange
            var reporters = new[] { Reporter.RealTimeDashboard };
            var options = new StrykerOptions
            {
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();
            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();
            var mutant = mutationTree.Mutants.First();

            // Act
            reporter.OnMutantTested(mutant);

            // Assert
            mockProcess.VerifyNoOtherCalls();
            dashboardClientMock.Verify(d => d.PublishMutantBatch(It.IsAny<JsonMutant>()));
        }

        [TestMethod]
        public void ShouldNotSendMutantsIfOpenDashboardOptionIsNotProvided()
        {
            // Arrange
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();
            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();
            var mutant = mutationTree.Mutants.First();

            // Act
            reporter.OnMutantTested(mutant);

            // Assert
            mockProcess.VerifyNoOtherCalls();
            dashboardClientMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShouldSendFinishedIfOpenDashboardOptionIsProvided()
        {
            // Arrange
            var reporters = new[] { Reporter.Dashboard };
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Dashboard,
                DashboardApiKey = "Access_Token",
                ProjectName = "github.com/JohnDoe/project",
                ProjectVersion = "version/human/readable",
                Reporters = reporters
            };
            var mockProcess = new Mock<IWebbrowserOpener>();
            var dashboardClientMock = new Mock<IDashboardClient>();
            var reporter = new DashboardReporter(options, dashboardClientMock.Object, browser: mockProcess.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            // Act
            reporter.OnAllMutantsTested(mutationTree, It.IsAny<TestProjectsInfo>());

            // Assert
            dashboardClientMock.Verify(d => d.PublishFinished());
        }
    }
}
