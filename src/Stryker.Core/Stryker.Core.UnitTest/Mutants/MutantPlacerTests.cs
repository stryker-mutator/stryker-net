using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantPlacerTests
    {
        [Fact]
        public void MutantPlacer_ShouldPlaceWithIfStatement()
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            var result = MutantPlacer.PlaceWithIfStatement(originalNode, mutatedNode, 0);

            result.ToFullString().ShouldBeSemantically(@"if (System.Environment.GetEnvironmentVariable(""ActiveMutation"") == ""0"")
            {
                1 - 8;
            } else {
                1 + 8;
            }");

            var removedResult = MutantPlacer.RemoveByIfStatement(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        [Fact]
        public void MutantPlacer_ShouldPlaceWithConditionalExpression()
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = MutantPlacer.PlaceWithConditionalExpression(originalNode, mutatedNode, 0);

            result.ToFullString().ShouldBeSemantically(@"System.Environment.GetEnvironmentVariable(""ActiveMutation"") == ""0"" ? 1 - 8 : 1 + 8;");

            var removedResult = MutantPlacer.RemoveByConditionalExpression(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }
    }
}
