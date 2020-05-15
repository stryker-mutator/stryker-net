using Moq;
using Shouldly;
using Stryker.Core.Baseline;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline
{
    public class DiskBaselineProviderTests
    {
        [Fact]
        public async Task ShouldWriteToDisk()
        {
            var options = new StrykerOptions(basePath: @"C:/Users/JohnDoe/Project/TestFolder");
            var fileSystemMock = new Mock<IFileSystem>();

            var sut = new DiskBaselineProvider(options, fileSystemMock.Object);

            await sut.Save(JsonReport.Build(options, JsonReportTestHelper.CreateProjectWith()), "version");

            fileSystemMock.Verify(mock => mock.File.CreateText(It.IsAny<string>()), Times.Once);
        }
    }
}
