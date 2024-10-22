using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stryker.Abstractions;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Clients;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;

namespace Stryker.Core.UnitTest.Baseline.Providers;

[TestClass]
public class DashboardBaselineProviderTests : TestBase
{
    [TestMethod]
    public async Task Load_Calls_DashboardClient_With_version()
    {
        var strykerOptions = new StrykerOptions();

        var dashboardClient = new Mock<IDashboardClient>();

        dashboardClient.Setup(x => x.PullReport(It.IsAny<string>()));

        var target = new DashboardBaselineProvider(strykerOptions, dashboardClient.Object);

        await target.Load("version");

        dashboardClient.Verify(client => client.PullReport(It.Is<string>(x => x == "version")), Times.Once);
    }

    [TestMethod]
    public async Task Save_Calls_DashboardClient_With_version()
    {
        var strykerOptions = new StrykerOptions();

        var dashboardClient = new Mock<IDashboardClient>();

        dashboardClient.Setup(x => x.PublishReport(It.IsAny<JsonReport>(), It.IsAny<string>(), false));

        var target = new DashboardBaselineProvider(strykerOptions, dashboardClient.Object);

        await target.Save(JsonReport.Build(strykerOptions, ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>()), "version");

        dashboardClient.Verify(client => client.PublishReport(It.IsAny<JsonReport>(), It.Is<string>(x => x == "version"), false), Times.Once);
    }
}
