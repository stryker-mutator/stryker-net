using System.Threading.Tasks;
using Moq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Clients;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers
{
    public class DashboardBaselineProviderTests
    {
        [Fact]
        public async Task Load_Calls_DashboardClient_With_version()
        {
            var strykerOptions = new StrykerOptions();

            var dashboardClient = new Mock<IDashboardClient>();

            dashboardClient.Setup(x => x.PullReport(It.IsAny<string>()));

            var target = new DashboardBaselineProvider(strykerOptions, dashboardClient.Object);

            await target.Load("version");

            dashboardClient.Verify(x => x.PullReport(It.Is<string>(x => x == "version")), Times.Once);
        }

        [Fact]
        public async Task Save_Calls_DashboardClient_With_version()
        {
            var strykerOptions = new StrykerOptions();

            var dashboardClient = new Mock<IDashboardClient>();

            dashboardClient.Setup(x => x.PublishReport(It.IsAny<string>(), It.IsAny<string>()));

            var target = new DashboardBaselineProvider(strykerOptions, dashboardClient.Object);

            await target.Save(JsonReport.Build(strykerOptions, JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent()), "version");

            dashboardClient.Verify(x => x.PublishReport(It.IsAny<string>(), It.Is<string>(x => x == "version")), Times.Once);
        }
    }
}
