using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime;

public class RealtimeMutantHandlerTest : TestBase
{
    private readonly Mock<ISseServer> _sseEventSenderMock;

    public RealtimeMutantHandlerTest() => _sseEventSenderMock = new Mock<ISseServer>();

    [Fact]
    public void ShouldOpenSseEndpoint()
    {
        var sut = new RealtimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.OpenSseEndpoint();

        _sseEventSenderMock.Verify(listener => listener.OpenSseEndpoint());
    }

    [Fact]
    public void ShouldWriteMessageToOutputStream()
    {
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
        var sut = new RealtimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.SendMutantTestedEvent(mutant);

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<JsonMutant>>(@event
                => @event.Data.Id == "1" && @event.Event == SseEventType.MutantTested)
            ));
    }

    [Fact]
    public void ShouldCloseSseEndpoint()
    {
        var sut = new RealtimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.CloseSseEndpoint();

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<string>>(@event
                => @event.Data.Length == 0 && @event.Event == SseEventType.Finished)
            ));
        _sseEventSenderMock.Verify(sse => sse.CloseSseEndpoint());
    }

    [Fact]
    public void ShouldSetPort()
    {
        var sut = new RealtimeMutantHandler(null, _sseEventSenderMock.Object);
        _sseEventSenderMock.Setup(s => s.Port).Returns(8080);

        sut.Port.ShouldBeEquivalentTo(8080);
    }
}
