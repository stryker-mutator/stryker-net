using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
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
        [Fact]
        public void Load_Report_Directory_NotFound()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(false, default));

            // Act
            var report = new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1").Result;

            // Assert
            report.ShouldBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();
        }

        [Fact]
        public void Load_Report_File_NotFound()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // version directory
            var subDirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subDirectoryClient);
            Mock.Get(subDirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // report file
            var fileClient = Mock.Of<ShareFileClient>();
            Mock.Get(subDirectoryClient).Setup(d => d.GetFileClient("stryker-report.json")).Returns(fileClient);
            Mock.Get(fileClient).Setup(f => f.ExistsAsync(default)).Returns(Task.FromResult(Response.FromValue(false, default)));

            // Act
            var report = new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1").Result;

            // Assert
            report.ShouldBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();

            Mock.Get(subDirectoryClient).VerifyAll();
            Mock.Get(subDirectoryClient).VerifyNoOtherCalls();

            Mock.Get(fileClient).VerifyAll();
            Mock.Get(fileClient).VerifyNoOtherCalls();
        }

        [Fact]
        public void Load_Report_Returns_Report()
        {
            // Arrange
            var shareClient = Mock.Of<ShareClient>();

            // root directory
            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);
            Mock.Get(directoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // version directory
            var subDirectoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(directoryClient).Setup(d => d.GetSubdirectoryClient("v1")).Returns(subDirectoryClient);
            Mock.Get(subDirectoryClient).Setup(d => d.Exists(default)).Returns(Response.FromValue(true, default));

            // report file
            var fileClient = Mock.Of<ShareFileClient>();
            Mock.Get(subDirectoryClient).Setup(d => d.GetFileClient("stryker-report.json")).Returns(fileClient);
            Mock.Get(fileClient).Setup(f => f.ExistsAsync(default)).Returns(Task.FromResult(Response.FromValue(true, default)));

            // report file content download
            var json = JsonReport.Build(new StrykerOptions(), ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>()).ToJson();
            var file = FilesModelFactory.StorageFileDownloadInfo(content: new MemoryStream(Encoding.Default.GetBytes(json)));
            Mock.Get(fileClient).Setup(f => f.Download(null, default)).Returns(Response.FromValue(file, default));

            // Act
            var report = new AzureFileShareBaselineProvider(new StrykerOptions(), shareClient).Load("v1").Result;

            // Assert
            report.ShouldNotBeNull();

            Mock.Get(shareClient).VerifyAll();
            Mock.Get(shareClient).VerifyNoOtherCalls();

            Mock.Get(directoryClient).VerifyAll();
            Mock.Get(directoryClient).VerifyNoOtherCalls();

            Mock.Get(subDirectoryClient).VerifyAll();
            Mock.Get(subDirectoryClient).VerifyNoOtherCalls();

            Mock.Get(fileClient).VerifyAll();
            Mock.Get(fileClient).VerifyNoOtherCalls();
        }

        [Fact]
        public void Save_Report_Small()
        {
            // Arrange

            // root directory
            var shareClient = Mock.Of<ShareClient>();

            var directoryClient = Mock.Of<ShareDirectoryClient>();
            Mock.Get(shareClient)
                .Setup(s => s.GetDirectoryClient("StrykerOutput"))
                .Returns(directoryClient);



            // Act

            // Assert
        }
    }
}
