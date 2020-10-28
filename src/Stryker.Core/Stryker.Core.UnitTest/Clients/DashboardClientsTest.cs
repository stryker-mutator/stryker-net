using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Shouldly;
using Stryker.Core.Clients;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Stryker.Core.UnitTest.Clients
{
    public class DashboardClientsTest
    {
        [Fact]
        public async Task DashboardClient_Logs_And_Returns_Null_On_Publish_Report_Does_Not_Return_200()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent("Error message", Encoding.UTF8, "text/html")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);


            var target = new DashboardClient(new StrykerOptions(
                dashboardUrl: "http://www.example.com/",
                dashboardApiKey: "Acces_Token"
                ), httpClient, loggerMock.Object);

            // Act
            var result = await target.PublishReport("string_json", "version");

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            result.ShouldBeNull();

        }

        [Fact]
        public async Task DashboardClient_Calls_With_Right_URL()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"Href\": \"http://www.example.com/api/projectName/version\"}", Encoding.UTF8, "text/html")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            var reporters = new String[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                dashboardUrl: "http://www.example.com",
                dashboardApiKey: "Acces_Token",
                projectName: "github.com/JohnDoe/project",
                projectVersion: "test/version",
                reporters: reporters
                );

            var target = new DashboardClient(options, httpClient, loggerMock.Object);


            // Act
            await target.PublishReport("string_json", "version");

            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task DashboardClient_Calls_With_Right_URL_With_Module_Appended()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"Href\": \"http://www.example.com/api/projectName/version\"}", Encoding.UTF8, "text/html")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            var reporters = new String[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                dashboardUrl: "http://www.example.com",
                dashboardApiKey: "Acces_Token",
                projectName: "github.com/JohnDoe/project",
                projectVersion: "test/version",
                reporters: reporters,
                moduleName: "moduleName"
                );

            var target = new DashboardClient(options, httpClient, loggerMock.Object);


            // Act
            await target.PublishReport("string_json", "version");

            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version?module=moduleName");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task DashboardClient_Get_With_Right_URL()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                dashboardUrl: "http://www.example.com",
                dashboardApiKey: "Acces_Token",
                projectName: "github.com/JohnDoe/project",
                projectVersion: "test/version",
                reporters: reporters
                );

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

            var target = new DashboardClient(options, httpClient, loggerMock.Object);

            // Act
            var result = await target.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ToJson().Equals(jsonReport.ToJson());
        }

        [Fact]
        public async Task DashboardClient_Get_With_Right_URL_with_module_name()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                dashboardUrl: "http://www.example.com",
                dashboardApiKey: "Acces_Token",
                projectName: "github.com/JohnDoe/project",
                projectVersion: "test/version",
                reporters: reporters,
                moduleName: "moduleName"
                );

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

            var target = new DashboardClient(options, httpClient, loggerMock.Object);

            // Act
            var result = await target.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version?module=moduleName");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ToJson().Equals(jsonReport.ToJson());
        }

        [Fact]
        public async Task DashboardClient_Get_Returns_Null_When_Statuscode_Not_200()
        {
            // Arrange 
            var loggerMock = new Mock<ILogger<DashboardClient>>(MockBehavior.Loose);
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var reporters = new string[] {
                "dashboard"
            };


            var options = new StrykerOptions(
                dashboardUrl: "http://www.example.com",
                dashboardApiKey: "Acces_Token",
                projectName: "github.com/JohnDoe/project",
                projectVersion: "test/version",
                reporters: reporters
                );


            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            var target = new DashboardClient(options, httpClient, loggerMock.Object);

            // Act
            var result = await target.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ShouldBeNull();
        }
    }
}
