using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime.Events;

public class SseEventTest : TestBase
{
    [Fact]
    public void ShouldFormatStringCorrectlyWithEmptyString()
    {
        var sut = new SseEvent<string> { EventName = "finished", Data = "" };

        sut.ToString().ShouldBeEquivalentTo("event: finished\ndata:\"\"\n\n");
    }

    [Fact]
    public void ShouldFormatStringCorrectlyWithMutantData()
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
        var jsonMutant = new JsonMutant(mutant);
        var sut = new SseEvent<JsonMutant> { EventName = "mutant-tested", Data = jsonMutant };

        sut.ToString().ShouldBeEquivalentTo("event: mutant-tested" + "\n" + @"data:{""id"":""1"",""mutatorName"":""test mutation"",""description"":null,""replacement"":""\u0022B\u0022"",""location"":{""start"":{""line"":1,""column"":1},""end"":{""line"":1,""column"":4}},""status"":""Killed"",""statusReason"":null,""static"":false,""coveredBy"":[],""killedBy"":null,""testsCompleted"":null,""duration"":null}" + "\n\n");
    }
}
