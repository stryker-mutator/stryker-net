using Stryker.Core.Baseline;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using System.Threading.Tasks;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline
{
    public class AzureBaselineProviderTests
    {
        [Fact]
        public async Task Test()
        {
            var sut = new AzureBaselineProvider();

            var report = JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith());

            await sut.Save(report, "version");

        }

        [Fact]
        public async Task TestGet()
        {
            var sut = new AzureBaselineProvider();

            var report = JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith());

            await sut.Load("version");

        }
    }
}
