using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class PrefixUnaryMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression)]
        [InlineData(SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression)]
        [InlineData(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression)]
        [InlineData(SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Single(result);
            Assert.True(result.First().ReplacementNode.IsKind(expected));
        }
        
        [Theory]
        [InlineData(SyntaxKind.BitwiseNotExpression)]
        [InlineData(SyntaxKind.LogicalNotExpression)]
        public void ShouldMutateAnRemove(SyntaxKind original)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Single(result);
            Assert.True(result.First().ReplacementNode.IsKind(SyntaxKind.NumericLiteralExpression));
        }
        
        [Theory]
        [InlineData(SyntaxKind.AddressOfExpression)]
        [InlineData(SyntaxKind.PointerIndirectionExpression)]
        public void ShouldNotMutate(SyntaxKind orginal)
        {
            var target = new PrefixUnaryMutator();
            
            var originalNode = SyntaxFactory.PrefixUnaryExpression(orginal, SyntaxFactory.IdentifierName("a"));
            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Empty(result);
        }
    }
}