using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutants;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantPlacerTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void MutantPlacer_ShouldPlaceWithIfStatement(int id)
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8))));

            var result = MutantPlacer.PlaceWithIfStatement(originalNode, mutatedNode, id);

            result.ToFullString().ShouldBeSemantically(@"if (System.Environment.GetEnvironmentVariable(""ActiveMutation"") == """ + id.ToString() + @""")
            {
                1 - 8;
            } else {
                1 + 8;
            }");

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }

        [Theory]
        [InlineData(10)]
        [InlineData(16)]
        public void MutantPlacer_ShouldPlaceWithConditionalExpression(int id)
        {
            // 1 + 8;
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            // 1 - 8;
            var mutatedNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = MutantPlacer.PlaceWithConditionalExpression(originalNode, mutatedNode, id);

            result.ToFullString().ShouldBeSemantically(@"(System.Environment.GetEnvironmentVariable(""ActiveMutation"") == """ + id.ToString() + ") ? 1 - 8 : 1 + 8);");

            var removedResult = MutantPlacer.RemoveMutant(result);

            removedResult.ToString().ShouldBeSemantically(originalNode.ToString());
        }
    }
}
