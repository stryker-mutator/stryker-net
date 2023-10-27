using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers
{
    public class AzureFileShareBaselineProviderTests : TestBase
    {
        private readonly string _uri = "https://strykernetbaseline.file.core.windows.net/baselines";
        [Fact]
        public async Task Authentication_Failure_Async()
        {
            // Arrange
            var logger = Mock.Of<ILogger<AzureFileShareBaselineProvider>>();

            var options = new StrykerOptions { AzureFileStorageUrl = _uri, AzureFileStorageSas = "sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2023-09-14T19:05:24Z&st=2023-09-14T11:05:24Z&spr=https&sig=hMf2EN3tD8T7y8Eei3aZASKdp5x%2BOkgEVIgTfxZPC38%3D" };

            // Act
            var report = await new AzureFileShareBaselineProvider(options, null, logger).Load("v1");

            // Assert
            report.ShouldBeNull();

            Mock.Get(logger).Verify(LogLevel.Warning, "Problem authenticating with azure file share. Make sure your SAS is valid.");
            Mock.Get(logger).Verify(LogLevel.Debug, $"No baseline was found at {options.AzureFileStorageUrl}/StrykerOutput/v1/stryker-report.json");
        }

        [Fact]
        public async Task Load_Report_Directory_NotFound()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();
            Mock.Get(shareClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));
            Mock.Get(shareClient).SetupGet(s => s.Uri).Returns(new Uri(_uri));

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // version directory
            var subdirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subdirectoryClient);
            Mock.Get(subdirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(false, default));

            // Act
            var report = await new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1");

            // Assert
            report.ShouldBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Load_Report_File_NotFound()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();
            Mock.Get(shareClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));
            Mock.Get(shareClient).SetupGet(s => s.Uri).Returns(new Uri(_uri));

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // version directory
            var subdirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subdirectoryClient);
            Mock.Get(subdirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // report file
            var fileClient = Mock.Of<ShareFileClient>();
            Mock.Get(subdirectoryClient).Setup(d => d.GetFileClient("stryker-report.json")).Returns(fileClient);
            Mock.Get(fileClient).Setup(f => f.ExistsAsync(default)).Returns(Task.FromResult(Response.FromValue(false, default)));

            // Act
            var report = await new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1");

            // Assert
            report.ShouldBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();

            Mock.Get(subdirectoryClient).VerifyAll();
            Mock.Get(subdirectoryClient).VerifyNoOtherCalls();

            Mock.Get(fileClient).VerifyAll();
            Mock.Get(fileClient).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Load_Report_Returns_Report()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();
            Mock.Get(shareClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));
            Mock.Get(shareClient).SetupGet(s => s.Uri).Returns(new Uri(_uri));

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // version directory
            var subdirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subdirectoryClient);
            Mock.Get(subdirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // report file
            var fileClient = Mock.Of<ShareFileClient>();
            Mock.Get(subdirectoryClient).Setup(d => d.GetFileClient("stryker-report.json")).Returns(fileClient);
            Mock.Get(fileClient).Setup(f => f.ExistsAsync(default)).Returns(Task.FromResult(Response.FromValue(true, default)));

            // report file content download
            var json = JsonReport.Build(new StrykerOptions(), ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>()).ToJson();
            var file = FilesModelFactory.StorageFileDownloadInfo(content: new MemoryStream(Encoding.Default.GetBytes(json)));
            Mock.Get(fileClient).Setup(f => f.Download(null, default)).Returns(Response.FromValue(file, default));

            // Act
            var report = await new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1");

            // Assert
            report.ShouldNotBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();

            Mock.Get(subdirectoryClient).VerifyAll();
            Mock.Get(subdirectoryClient).VerifyNoOtherCalls();

            Mock.Get(fileClient).VerifyAll();
            Mock.Get(fileClient).VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(2, 5)]
        [InlineData(20, 200)]
        [InlineData(100, 500)]
        public async Task Save_Report(int folders, int files)
        {
            var chunkSize = 4194304;

            // Arrange
            var shareClient = Mock.Of<ShareClient>();
            var logger = Mock.Of<ILogger<AzureFileShareBaselineProvider>>();

            Mock.Get(shareClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));
            Mock.Get(shareClient).SetupGet(s => s.Uri).Returns(new Uri(_uri));

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.CreateIfNotExists(default, default, default, default))
                .Callback(() => Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default)));

            // version directory
            var subdirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subdirectoryClient);
            Mock.Get(subdirectoryClient)
                .Setup(d => d.CreateIfNotExists(default, default, default, default))
                .Callback(() => Mock.Get(subdirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default)));

            // report file
            var report = JsonReport.Build(new StrykerOptions(), ReportTestHelper.CreateProjectWith(folders: folders, files: files), It.IsAny<TestProjectsInfo>());
            var fileLength = Encoding.UTF8.GetBytes(report.ToJson()).Length;

            var fullChunks = (int)Math.Floor((double)fileLength / chunkSize);
            var lastChunkSize = fileLength - (fullChunks * chunkSize);

            var fileClient = Mock.Of<ShareFileClient>();

            Mock.Get(subdirectoryClient).Setup(d => d.GetFileClient("stryker-report.json")).Returns(fileClient);
            Mock.Get(fileClient)
                .Setup(f => f.CreateAsync(fileLength, default, default, default, default, default, default))
                .Returns(Task.FromResult(Response.FromValue(Mock.Of<ShareFileInfo>(), Mock.Of<Response>())));

            if (fullChunks > 0)
            {
                // setup full chunks upload
                for (var i = 0; i < fullChunks; i++)
                {
                    var offset = i * chunkSize;
                    Mock.Get(fileClient)
                        .Setup(f => f.UploadRangeAsync(
                            It.Is<HttpRange>(r => r.Offset == offset && r.Length == chunkSize),
                            It.IsAny<Stream>(), null, default))
                        .Returns(Task.FromResult(Response.FromValue(Mock.Of<ShareFileUploadInfo>(), Mock.Of<Response>())));
                }

                // setup last chunk upload
                Mock.Get(fileClient)
                    .Setup(f => f.UploadRangeAsync(
                        It.Is<HttpRange>(r => r.Offset == fullChunks * chunkSize && r.Length == lastChunkSize),
                        It.IsAny<Stream>(), null, default))
                    .Returns(Task.FromResult(Response.FromValue(Mock.Of<ShareFileUploadInfo>(), Mock.Of<Response>())));
            }
            else // There's only 1 chunk
            {
                Mock.Get(fileClient)
                    .Setup(f => f.UploadRangeAsync(
                        It.Is<HttpRange>(r => r.Offset == 0 && r.Length == fileLength),
                        It.IsAny<Stream>(), null, default))
                    .Returns(Task.FromResult(Response.FromValue(Mock.Of<ShareFileUploadInfo>(), Mock.Of<Response>())));
            }

            // Act
            await new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient, logger).Save(report, "v1");

            // Assert
            Mock.Get(logger).Verify(LogLevel.Debug, $"Uploaded report chunk {fileLength}/{fileLength} to azure file share");

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();

            Mock.Get(subdirectoryClient).VerifyAll();
            Mock.Get(subdirectoryClient).VerifyNoOtherCalls();

            Mock.Get(fileClient).VerifyAll();
            Mock.Get(fileClient).VerifyNoOtherCalls();
        }
    }
}
