using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers;

public class DiskBaselineProviderTests : TestBase
{
    [Fact]
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

    [Fact]
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

    [Fact]
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
