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
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Providers;

public class AzureFileShareBaselineProviderTests : TestBase
{
    [Theory]
    [InlineData("sv=AZURE_SAS_KEY")]
    [InlineData("?sv=AZURE_SAS_KEY")]
    public async Task Load_Calls_Correct_URL(string sas)
    {
        // Arrange
        var options = new StrykerOptions()
        {
            AzureFileStorageUrl = "https://www.filestoragelocation.com",
            AzureFileStorageSas = sas,
            BaselineProvider = BaselineProvider.AzureFileStorage
        };

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

        var jsonReport = JsonReport.Build(options, readonlyInputComponent, It.IsAny<TestProjectsInfo>());

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

        var result = await target.Load("baseline/project_version");

        var expectedUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

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
        var options = new StrykerOptions()
        {
            AzureFileStorageUrl = "https://www.filestoragelocation.com",
            AzureFileStorageSas = "sv=AZURE_SAS_KEY",
            BaselineProvider = BaselineProvider.AzureFileStorage
        };

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

        var jsonReport = JsonReport.Build(options, readonlyInputComponent, It.IsAny<TestProjectsInfo>());

        var expectedGetUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedCreateDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/project_version?sv=AZURE_SAS_KEY&restype=directory");

        var expectedFileAllocationUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/project_version/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedUploadContentUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/project_version/stryker-report.json?sv=AZURE_SAS_KEY&comp=range");

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

        await target.Save(jsonReport, "baseline/project_version");

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
    public async Task Save_Calls_CreateDictionaryWithProjectName_And_AllocateFileLocation_When_Baseline_Does_Not_Exists()
    {
        // arrange
        var projectName = "my-project-name";
        var projectVersion = "baseline/my-project-version";

        var options = new StrykerOptions
        {
            AzureFileStorageUrl = "https://www.filestoragelocation.com",
            AzureFileStorageSas = "sv=AZURE_SAS_KEY",
            WithBaseline = false,
            ProjectName = projectName
        };

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

        var jsonReport = JsonReport.Build(options, readonlyInputComponent, It.IsAny<TestProjectsInfo>());

        var expectedGetUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/{projectVersion}/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedCreateOutputDirectoryUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/?sv=AZURE_SAS_KEY&restype=directory");
        var expectedCreateProjectOutputDirectoryUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/?sv=AZURE_SAS_KEY&restype=directory");
        var expectedCreateBaselinesDirectoryUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/baseline/?sv=AZURE_SAS_KEY&restype=directory");
        var expectedCreateVersionDirectoryUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/{projectVersion}/?sv=AZURE_SAS_KEY&restype=directory");

        var expectedFileAllocationUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/{projectVersion}/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedUploadContentUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{projectName}/{projectVersion}/stryker-report.json?sv=AZURE_SAS_KEY&comp=range");

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedGetUri && requestMessage.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Content = new StringContent(jsonReport.ToJson(), Encoding.UTF8, "application/json")
            })
            .Verifiable();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedCreateOutputDirectoryUri && requestMessage.Method == HttpMethod.Put),
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
            ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == expectedCreateProjectOutputDirectoryUri && requestMessage.Method == HttpMethod.Put),
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

        await target.Save(jsonReport, projectVersion);

        // assert
        handlerMock.VerifyAll();
    }

    [Theory]
    [InlineData("baseline/2.0.0")]
    [InlineData("baseline/2.0.0-beta001")]
    [InlineData("baseline/master")]
    [InlineData("baseline/project_version")]
    public async Task Save_Calls_CreateDictionary_And_AllocateFileLocation_When_Baseline_Does_Not_Exists(string version)
    {
        // arrange
        var options = new StrykerOptions()
        {
            AzureFileStorageUrl = "https://www.filestoragelocation.com",
            AzureFileStorageSas = "sv=AZURE_SAS_KEY",
            BaselineProvider = BaselineProvider.AzureFileStorage
        };
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;

        var jsonReport = JsonReport.Build(options, readonlyInputComponent, It.IsAny<TestProjectsInfo>());

        var expectedGetUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{version}/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedCreateStrykerOutputDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/?sv=AZURE_SAS_KEY&restype=directory");
        var expectedCreateBaselinesDirectoryUri = new Uri("https://www.filestoragelocation.com/StrykerOutput/baseline/?sv=AZURE_SAS_KEY&restype=directory");
        var expectedCreateVersionDirectoryUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{version}/?sv=AZURE_SAS_KEY&restype=directory");

        var expectedFileAllocationUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{version}/stryker-report.json?sv=AZURE_SAS_KEY");

        var expectedUploadContentUri = new Uri($"https://www.filestoragelocation.com/StrykerOutput/{version}/stryker-report.json?sv=AZURE_SAS_KEY&comp=range");

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

        await target.Save(jsonReport, version);

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
