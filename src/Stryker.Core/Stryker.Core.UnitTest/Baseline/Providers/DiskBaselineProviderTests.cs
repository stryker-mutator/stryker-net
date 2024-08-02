using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Stryker.Configuration.Baseline.Providers;
using Stryker.Configuration;
using Stryker.Configuration.ProjectComponents.TestProjects;
using Stryker.Configuration.Reporters.Json;
using Stryker.Configuration.UnitTest.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Baseline.Providers
{
    [TestClass]
    public class DiskBaselineProviderTests : TestBase
    {
        [TestMethod]
        public async Task ShouldWriteToDiskAsync()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();
            var options = new StrykerOptions()
            {
                ProjectPath = @"C:/Users/JohnDoe/Project/TestFolder"
            };
            var sut = new DiskBaselineProvider(options, fileSystemMock);

            // Act
            await sut.Save(JsonReport.Build(options, ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>()), "baseline/version");

            // Assert
            var path = FilePathUtils.NormalizePathSeparators(@"C:/Users/JohnDoe/Project/TestFolder/StrykerOutput/baseline/version/stryker-report.json");

            var file = fileSystemMock.GetFile(path);
            file.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task ShouldHandleFileNotFoundExceptionOnLoadAsync()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();
            var options = new StrykerOptions { ProjectPath = "C:/Dev" };
            var sut = new DiskBaselineProvider(options, fileSystemMock);

            // Act
            var result = await sut.Load("testversion");

            result.ShouldBeNull();
        }

        [TestMethod]
        public async Task ShouldLoadReportFromDiskAsync()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();
            var options = new StrykerOptions()
            {
                ProjectPath = @"C:/Users/JohnDoe/Project/TestFolder"
            };
            var report = JsonReport.Build(options, ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>());

            fileSystemMock.AddFile("C:/Users/JohnDoe/Project/TestFolder/StrykerOutput/baseline/version/stryker-report.json", report.ToJson());

            var target = new DiskBaselineProvider(options, fileSystemMock);

            // Act
            var result = await target.Load("baseline/version");

            // Assert
            result.ShouldNotBeNull();
            result.ToJson().ShouldBe(report.ToJson());
        }
    }
}
