using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Shouldly;
using Stryker.Core.Clients;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.UnitTest.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Clients
{
    public class DashboardClientsTest : TestBase
    {
        private static readonly StrykerOptions OptionsWithoutModule = new()
        {
            DashboardUrl = "http://www.example.com",
            DashboardApiKey = "Access_Token",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "test/version",
            Reporters = new [] { Reporter.Dashboard },
        };

        private static readonly StrykerOptions OptionsWithModule = new()
        {
            DashboardUrl = "http://www.example.com",
            DashboardApiKey = "Access_Token",
            ModuleName = "testModule",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "test/version",
            Reporters = new[] { Reporter.Dashboard },
        };

        private static readonly StrykerOptions OptionsWithEmptyModule = new()
        {
            DashboardUrl = "http://www.example.com",
            DashboardApiKey = "Access_Token",
            ModuleName = "",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "test/version",
            Reporters = new[] { Reporter.Dashboard },
        };

        private static readonly JsonMutant Mutant = new(new Mutant
        {
            Id = 1,
            Mutation = new Mutation
            {
                DisplayName = "test mutation",
                OriginalNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("A"))),
                ReplacementNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("B"))),
            },
            ResultStatus = MutantStatus.Killed,
        });

        private readonly Mock<ILogger<DashboardClient>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _handlerMock;

        private DashboardClient _sut;

        public DashboardClientsTest()
        {
            _loggerMock = new Mock<ILogger<DashboardClient>>();
            _handlerMock = new Mock<HttpMessageHandler>();
            _sut = new DashboardClient(OptionsWithoutModule, new HttpClient(_handlerMock.Object), _loggerMock.Object);
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingReport_ShouldLogAndReturnNullWhenApiDoesNotReturn200()
        {
            // Arrange
            ArrangeHandlerReturnsBadRequest();

            var options = new StrykerOptions
            {
                DashboardUrl = "http://www.example.com/",
                DashboardApiKey = "Access_Token"
            };
            var sut = new DashboardClient(options, new HttpClient(_handlerMock.Object), _loggerMock.Object);

            // Act
            var result = await sut.PublishReport(new MockJsonReport(null, null), "version");


            // Assert
            VerifyErrorLogged();
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingReport_ShouldCallWithTheCorrectUri()
        {
            // Arrange
            const string Href = $"{{\"Href\": \"http://www.example.com/api/projectName/version\"}}";
            ArrangeHandlerReturnsOk(Href);

            // Act
            var result = await _sut.PublishReport(new MockJsonReport(null, null), "version");

            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ShouldBe("http://www.example.com/api/projectName/version");
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingReport_WithModule_ShouldCallTheCorrectUri()
        {
            // Arrange
            const string Href = $"{{\"Href\": \"http://www.example.com/api/projectName/version\"}}";
            ArrangeHandlerReturnsOk(Href);

            var sut = new DashboardClient(OptionsWithModule, new HttpClient(_handlerMock.Object), _loggerMock.Object);

            // Act
            var result = await sut.PublishReport(new MockJsonReport(null, null), "version");

            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version?module=testModule");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ShouldBe("http://www.example.com/api/projectName/version");
        }

        [Fact]
        public async Task DashboardClient_ShouldNotAppendModuleIfOptionIsAnEmptyString()
        {
            // Arrange
            const string Href = $"{{\"Href\": \"http://www.example.com/api/projectName/version\"}}";
            ArrangeHandlerReturnsOk(Href);

            var sut = new DashboardClient(OptionsWithEmptyModule, new HttpClient(_handlerMock.Object), _loggerMock.Object);

            // Act
            var result = await sut.PublishReport(new MockJsonReport(null, null), "version");

            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            result.ShouldBe("http://www.example.com/api/projectName/version");
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingRealTimeReport_ShouldCallTheCorrectUri()
        {
            // Arrange & Act
            await _sut.PublishReport(new MockJsonReport(null, null), "version", true);

            // Assert
            var expected = new Uri("http://www.example.com/api/real-time/github.com/JohnDoe/project/version");
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(message =>
                    message.Method == HttpMethod.Put &&
                    message.RequestUri == expected),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingRealTimeReport_WithModule_ShouldCallTheCorrectUri()
        {
            // Arrange
            _sut = new DashboardClient(OptionsWithModule, new HttpClient(_handlerMock.Object), _loggerMock.Object);

            // Act
            await _sut.PublishReport(new MockJsonReport(null, null), "version", true);

            // Assert
            var expected = new Uri("http://www.example.com/api/real-time/github.com/JohnDoe/project/version?module=testModule");
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(message =>
                    message.Method == HttpMethod.Put &&
                    message.RequestUri == expected),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task DashboardClient_WhilePullingReport_ShouldCallWithTheCorrectUri()
        {
            // Arrange
            var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;
            var jsonReport = JsonReport.Build(OptionsWithoutModule, readonlyInputComponent, It.IsAny<TestProjectsInfo>());
            var json = jsonReport.ToJson();

            ArrangeHandlerReturnsOk(json);

            // Act
            var result = await _sut.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ShouldNotBeNull();
            result.ToJson().ShouldBe(json);
        }

        [Fact]
        public async Task DashboardClient_WhilePullingReport_WithModule_ShouldCallWithTheCorrectUri()
        {
            // Arrange
            var readonlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose).Object;
            var jsonReport = JsonReport.Build(OptionsWithModule, readonlyInputComponent, It.IsAny<TestProjectsInfo>());
            var json = jsonReport.ToJson();
            var sut = new DashboardClient(OptionsWithModule, new HttpClient(_handlerMock.Object), _loggerMock.Object);

            ArrangeHandlerReturnsOk(json);

            // Act
            var result = await sut.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version?module=testModule");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                    ),
                ItExpr.IsAny<CancellationToken>()
                );

            result.ShouldNotBeNull();
            result.ToJson().ShouldBe(json);
        }

        [Fact]
        public async Task DashboardClient_WhilePullingReport_ShouldLogAndReturnNullWhenApiDoesNotReturn200()
        {
            // Arrange
            ArrangeHandlerReturnsBadRequest();

            // Act
            var result = await _sut.PullReport("version");

            // Assert
            var expectedUri = new Uri("http://www.example.com/api/reports/github.com/JohnDoe/project/version");

            _handlerMock.Protected().Verify(
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

        [Fact]
        public async Task DashboardClient_WhilePublishingBatchOnce_ShouldNotCallApi()
        {
            // Arrange & Act
            await _sut.PublishMutantBatch(Mutant);

            // Assert
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DashBoardClient_WhilePublishingBatch_ShouldCallWithTheCorrectUri()
        {
            // Arrange
            ArrangeHandlerReturnsOk();

            // Act
            for (var i = 0; i <= 10; i++)
            {
                await _sut.PublishMutantBatch(Mutant);
            }

            // Assert
            var expected = new Uri("http://www.example.com/api/real-time/github.com/JohnDoe/project/test/version");
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(message =>
                    message.Method == HttpMethod.Post &&
                    message.RequestUri == expected),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingBatch_ShouldLogErrorWhenApiDoesNotReturn200()
        {
            // Arrange
            ArrangeHandlerReturnsBadRequest();

            // Act
            for (var i = 0; i <= 10; i++)
            {
                await _sut.PublishMutantBatch(Mutant);
            }

            // Assert
            VerifyErrorLogged();
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingFinishedEvent_ShouldLogErrorWhenApiDoesNotReturn200()
        {
            // Arrange
            ArrangeHandlerReturnsBadRequest();

            // Act
            await _sut.PublishFinished();

            // Assert
            VerifyErrorLogged();
        }

        [Fact]
        public async Task DashboardClient_WhilePublishingFinishedEvent_ShouldEmptyBatchBeforeSendingFinished()
        {
            // Arrange
            ArrangeHandlerReturnsOk();

            // Act
            await _sut.PublishMutantBatch(Mutant);
            await _sut.PublishFinished();

            // Assert
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post
                ),
                ItExpr.IsAny<CancellationToken>()
            );
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        private void ArrangeHandlerReturnsOk(string data = "") =>
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                })
                .Verifiable();

        private void ArrangeHandlerReturnsBadRequest() =>
            _handlerMock
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

        private void VerifyErrorLogged() =>
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()));
    }
}
