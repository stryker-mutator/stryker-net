﻿using Shouldly;
using Stryker.Core.Baseline;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline
{
    public class DiskBaselineProviderTests
    {
        [Fact]
        public async Task ShouldWriteToDisk()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();
            var options = new StrykerOptions(basePath: @"C:/Users/JohnDoe/Project/TestFolder", fileSystem: fileSystemMock);
            var sut = new DiskBaselineProvider(options, fileSystemMock);

            // Act
            await sut.Save(JsonReport.Build(options, JsonReportTestHelper.CreateProjectWith()), "version");

            // Assert
            var path = FilePathUtils.NormalizePathSeparators(@"C:/Users/JohnDoe/Project/TestFolder/StrykerOutput/Baselines/version/stryker-report.json");

            MockFileData file = fileSystemMock.GetFile(path);
            file.ShouldNotBeNull();
        }

        [Fact]
        public async Task ShouldHandleFileNotFoundExceptionOnLoad()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();

            var sut = new DiskBaselineProvider(new StrykerOptions(), fileSystemMock);

            // Act
            var result = await sut.Load("testversion");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ShouldLoadReportFromDisk()
        {
            // Arrange
            var fileSystemMock = new MockFileSystem();
            var options = new StrykerOptions(basePath: @"C:/Users/JohnDoe/Project/TestFolder", fileSystem: fileSystemMock);

            var report = JsonReport.Build(options, JsonReportTestHelper.CreateProjectWith());

            fileSystemMock.AddFile("C:/Users/JohnDoe/Project/TestFolder/StrykerOutput/Baselines/version/stryker-report.json", report.ToJson());

            var target = new DiskBaselineProvider(options, fileSystemMock);

            // Act
            var result = await target.Load("version");

            // Assert
            result.ShouldNotBeNull();
            result.ToJson().ShouldBe(report.ToJson());
        }
    }
}
