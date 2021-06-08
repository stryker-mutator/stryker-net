using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class StringMutatorTests
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new StringMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("", "Stryker was here!")]
        [InlineData("foo", "")]
        public void ShouldMutate(string original, string expected)
        {
            var node = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(original));
            var mutator = new StringMutator();

            var result = mutator.ApplyMutations(node).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
                .Token.Value.ShouldBe(expected);
            mutation.DisplayName.ShouldBe("String mutation");
        }
    }
}
