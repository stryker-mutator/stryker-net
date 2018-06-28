using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class LogicalMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)]
        [InlineData(SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new LogicalMutator();

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
