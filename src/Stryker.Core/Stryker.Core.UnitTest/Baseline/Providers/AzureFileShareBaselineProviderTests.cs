using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers
{
    public class AzureFileShareBaselineProviderTests
    {
        [Fact]
        public async Task Load_Calls_Correct_URL()
        {
            // Arrange
            var options = new StrykerOptions(azureFileStorageUrl: "https://www.filestoragelocation.com", azureSAS: "AZURE_SAS_KEY", baselineStorageLocation: "azurefilestorage");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

            var jsonReport = JsonReport.Build(options, readonlyInputComponent);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(jsonReport.ToJson(), Encoding.UTF8, "application/json")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            var target = new AzureFileShareBaselineProvider(options, httpClient: httpClient);

            var result = await target.Load("project_version");

            var expectedUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

            handlerMock
                .Protected()
                .Verify(
                 "SendAsync",
                 Times.Exactly(1),
                 ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                 );
        }

        [Fact]
        public async Task Save_Doesnt_Call_CreateDictionary_And_AllocateFileLocation_When_Baseline_Exists()
        {
            // arrange
            var options = new StrykerOptions(azureFileStorageUrl: "https://www.filestoragelocation.com", azureSAS: "AZURE_SAS_KEY", baselineStorageLocation: "azurefilestorage");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

            var jsonReport = JsonReport.Build(options, readonlyInputComponent);

            var expectedGetUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

            var expectedCreateDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version?restype=directory&sv=AZURE_SAS_KEY");

            var expectedFileAllocationUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

            var expectedUploadContentUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?comp=range&sv=AZURE_SAS_KEY");

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedGetUri && requestMessage.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(jsonReport.ToJson(), Encoding.UTF8, "application/json")
                })
                .Verifiable();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedFileAllocationUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Content = new StringContent("Nothing went wrong", Encoding.UTF8, "application/json")
                })
                .Verifiable();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedUploadContentUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("Nothing went wrong", Encoding.UTF8, "application/json")
                })
                .Verifiable();

            var target = new AzureFileShareBaselineProvider(options, new HttpClient(handlerMock.Object));

            await target.Save(jsonReport, "project_version");

            // assert
            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Get
                   && req.RequestUri == expectedGetUri
                   ),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Put
                   && req.RequestUri == expectedCreateDirectoryUri
                   ),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
              .Protected()
              .Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put
                  && req.RequestUri == expectedFileAllocationUri
                  && req.Headers.Contains("x-ms-type")
                  ),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
              .Protected()
              .Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put
                  && req.RequestUri == expectedUploadContentUri),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task Save_Calls_CreateDictionary_And_AllocateFileLocation_When_Baseline_Does_Not_Exists()
        {
            // arrange
            var options = new StrykerOptions(azureFileStorageUrl: "https://www.filestoragelocation.com", azureSAS: "AZURE_SAS_KEY", baselineStorageLocation: "azurefilestorage");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

            var jsonReport = JsonReport.Build(options, readonlyInputComponent);

            var expectedGetUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

            var expectedCreateStrykerOutputDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/?restype=directory&sv=AZURE_SAS_KEY");
            var expectedCreateBaselinesDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/?restype=directory&sv=AZURE_SAS_KEY");
            var expectedCreateVersionDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/?restype=directory&sv=AZURE_SAS_KEY");

            var expectedFileAllocationUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

            var expectedUploadContentUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/Baselines/project_version/stryker-report.json?comp=range&sv=AZURE_SAS_KEY");

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedGetUri && requestMessage.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Content = new StringContent(jsonReport.ToJson(), Encoding.UTF8, "application/json")
                })
                .Verifiable();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedCreateStrykerOutputDirectoryUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created
                })
                .Verifiable();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedCreateBaselinesDirectoryUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created
                })
                .Verifiable();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedCreateVersionDirectoryUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created
                })
                .Verifiable();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedFileAllocationUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created
                })
                .Verifiable();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedUploadContentUri && requestMessage.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Content = new StringContent("File created")
                })
                .Verifiable();

            var target = new AzureFileShareBaselineProvider(options, new HttpClient(handlerMock.Object));

            await target.Save(jsonReport, "project_version");

            // assert
            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Get
                   && req.RequestUri == expectedGetUri),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Put
                   && req.RequestUri == expectedCreateStrykerOutputDirectoryUri),
                ItExpr.IsAny<CancellationToken>());
            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Put
                   && req.RequestUri == expectedCreateBaselinesDirectoryUri),
                ItExpr.IsAny<CancellationToken>());
            handlerMock
               .Protected()
               .Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Put
                   && req.RequestUri == expectedCreateVersionDirectoryUri),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
              .Protected()
              .Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put
                  && req.RequestUri == expectedFileAllocationUri
                  && req.Headers.Contains("x-ms-type")),
                ItExpr.IsAny<CancellationToken>());

            handlerMock
              .Protected()
              .Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put
                  && req.RequestUri == expectedUploadContentUri),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
