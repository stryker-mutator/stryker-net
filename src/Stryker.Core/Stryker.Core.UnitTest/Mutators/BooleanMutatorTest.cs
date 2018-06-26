using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class BooleanMutatorTest
    {
        [Theory]
        [InlineData(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression)]
        [InlineData(SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original)).ToList();

            Assert.Single(result);

            Assert.True(result.First().ReplacementNode.IsKind(expected));
        }

        [Theory]
        [InlineData(SyntaxKind.NumericLiteralExpression)]
        [InlineData(SyntaxKind.StringLiteralExpression)]
        [InlineData(SyntaxKind.CharacterLiteralExpression)]
        [InlineData(SyntaxKind.NullLiteralExpression)]
        [InlineData(SyntaxKind.DefaultLiteralExpression)]
        public void ShouldNotMutate(SyntaxKind orginal)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(orginal)).ToList();

            Assert.Empty(result);
        }
    }
}
