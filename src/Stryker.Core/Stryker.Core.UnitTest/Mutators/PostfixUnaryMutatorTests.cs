using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class PostfixUnaryMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression)]
        [InlineData(SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new PostfixUnaryMutator();
            var originalNode = SyntaxFactory.PostfixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Single(result);
            Assert.True(result.First().ReplacementNode.IsKind(expected));
        }
    }
}