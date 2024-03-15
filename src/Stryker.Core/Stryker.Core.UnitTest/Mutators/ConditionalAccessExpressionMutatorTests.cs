using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class ConditionalAccessExpressionMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new ConditionalAccessExpressionMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Theory]
    [InlineData("a?.b", "a.b")]
    [InlineData("a?.b?.c?.d", "a.b?.c?.d")]
    [InlineData("a.b?.c?.d", "a.b.c?.d")]
    [InlineData("a?.b()", "a.b()")]
    [InlineData("a?.b()?.c()", "a.b()?.c()")]
    [InlineData("a?.b?.c()", "a.b?.c()")]
    [InlineData("a?.b()?.c", "a.b()?.c")]
    [InlineData("a.b()?.c()", "a.b().c()")]
    [InlineData("a.b?.c()", "a.b.c()")]
    [InlineData("a.b()?.c", "a.b().c")]
    public void ShouldMutate(string original, string expected)
    {
        var target = new ConditionalAccessExpressionMutator();

        var expressionToMutate = SyntaxFactory.ParseExpression(original);

        var result = target.ApplyMutations(expressionToMutate as ConditionalAccessExpressionSyntax, null).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ToString().ShouldBeSemantically(expected);
        mutation.DisplayName.ShouldBe("Conditional access expression");
    }
}
