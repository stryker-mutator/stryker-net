using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ConditionalExpressionMutatorTests
    {
        [Fact]
        public void ShouldMutate()
        {
            var expression = "true ? 1 : 2";
            var inputExpressionSyntax = SyntaxFactory.ParseExpression(expression) as ConditionalExpressionSyntax;
            var target = new ConditionalExpressionMutator();

            var result = target.ApplyMutations(inputExpressionSyntax).ToList();

            result.ShouldHaveSingleItem();
            var outputExpressionSyntax = result.First().ReplacementNode as ConditionalExpressionSyntax;
            outputExpressionSyntax.ShouldNotBeNull();
            outputExpressionSyntax.WhenTrue.ToString().ShouldBe("2");
            outputExpressionSyntax.WhenFalse.ToString().ShouldBe("1");
        }
    }
}
