using System;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Html.RealTime;
using Stryker.Core.Reporters.Html.RealTime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Reporters.Html.RealTime;

[TestClass]
public class RealTimeMutantHandlerTest : TestBase
{
    private readonly Mock<ISseServer> _sseEventSenderMock = new();

    [TestMethod]
    public void ShouldOpenSseEndpoint()
    {
        var sut = new RealTimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.OpenSseEndpoint();

        _sseEventSenderMock.Verify(listener => listener.OpenSseEndpoint());
    }

    [TestMethod]
    public void ShouldWriteMessageToOutputStream()
    {
        _sseEventSenderMock.Setup(sse => sse.HasConnectedClients).Returns(true);
        var mutant = new Mutant
        {
            Id = 1,
            Mutation = new Mutation
            {
                DisplayName = "test mutation",
                OriginalNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("A"))),
                ReplacementNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("B"))),
            },
            ResultStatus = MutantStatus.Killed,
        };
        var sut = new RealTimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.SendMutantTestedEvent(mutant);

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<JsonMutant>>(@event
                => @event.Data.Id == "1" && @event.Event == SseEventType.MutantTested)
            ));
    }

    [TestMethod]
    public void ShouldCloseSseEndpoint()
    {
        _sseEventSenderMock.Setup(sse => sse.HasConnectedClients).Returns(true);
        var sut = new RealTimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.CloseSseEndpoint();

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<string>>(@event
                => @event.Data.Length == 0 && @event.Event == SseEventType.Finished)
            ));
        _sseEventSenderMock.Verify(sse => sse.CloseSseEndpoint());
    }

    [TestMethod]
    public void ShouldSetPort()
    {
        _sseEventSenderMock.Setup(sse => sse.HasConnectedClients).Returns(true);
        var sut = new RealTimeMutantHandler(null, _sseEventSenderMock.Object);
        _sseEventSenderMock.Setup(s => s.Port).Returns(8080);

        sut.Port.ShouldBeEquivalentTo(8080);
    }

    [TestMethod]
    public void ShouldQueueEventsUntilAtleastOneClientIsConnected()
    {
        _sseEventSenderMock.Setup(sse => sse.HasConnectedClients).Returns(false);
        var sut = new RealTimeMutantHandler(null, _sseEventSenderMock.Object);

        var mutant = new Mutant
        {
            Id = 1,
            Mutation = new Mutation
            {
                DisplayName = "test mutation",
                OriginalNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("A"))),
                ReplacementNode = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("B"))),
            },
            ResultStatus = MutantStatus.Killed,
        };
        sut.SendMutantTestedEvent(mutant);

        // Verify that the event is not called yet.

        _sseEventSenderMock.Verify(service => service.SendEvent(It.IsAny<SseEvent<JsonMutant>>()), Times.Never);

        // Connect a client.
        _sseEventSenderMock.Raise(server => server.ClientConnected += null, EventArgs.Empty);

        // Verify that the event is sent after a client connects.

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.IsAny<SseEvent<JsonMutant>>()));
    }
}
