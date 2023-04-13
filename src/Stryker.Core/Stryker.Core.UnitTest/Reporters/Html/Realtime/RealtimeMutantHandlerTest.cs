﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime;

public class RealtimeMutantHandlerTest : TestBase
{
    private readonly Mock<ISseEventSender> _sseEventSenderMock;

    public RealtimeMutantHandlerTest() => _sseEventSenderMock = new Mock<ISseEventSender>();

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

        sut.SendMutantResultEvent(mutant);

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<JsonMutant>>(@event
                => @event.Data.Id == "1" && @event.Type == SseEventType.Mutation)
            ));
    }

    [Fact]
    public void ShouldCloseSseEndpoint()
    {
        var sut = new RealtimeMutantHandler(null, _sseEventSenderMock.Object);

        sut.CloseSseEndpoint();

        _sseEventSenderMock.Verify(sse
            => sse.SendEvent(It.Is<SseEvent<string>>(@event
                => @event.Data.Length == 0 && @event.Type == SseEventType.Finished)
            ));
        _sseEventSenderMock.Verify(sse => sse.CloseSseEndpoint());
    }
}
