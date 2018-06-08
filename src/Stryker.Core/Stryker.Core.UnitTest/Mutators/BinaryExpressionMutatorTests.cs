using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class BinaryExpressionMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.AddExpression, SyntaxKind.SubtractExpression)]
        [InlineData(SyntaxKind.SubtractExpression, SyntaxKind.AddExpression)]
        [InlineData(SyntaxKind.MultiplyExpression, SyntaxKind.DivideExpression)]
        [InlineData(SyntaxKind.DivideExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(SyntaxKind.ModuloExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression)]
        [InlineData(SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression)]
        [InlineData(SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression)]
        [InlineData(SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression)]
        [InlineData(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)]
        [InlineData(SyntaxKind.NotEqualsExpression, SyntaxKind.EqualsExpression)]
        [InlineData(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)]
        [InlineData(SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression)]
        public void MathMutator_ShouldMutate(SyntaxKind input, SyntaxKind expectedOutput, SyntaxKind optionalExpectedOutput = SyntaxKind.None)
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(input,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode).ToList();

            if(optionalExpectedOutput == SyntaxKind.None)
            {
                // there should be only one mutation
                result.ShouldHaveSingleItem();
                result.ShouldContain(x => x.ReplacementNode.IsKind(expectedOutput));
            } else
            {
                // there should be two mutations
                result.Count.ShouldBe(2, "Two mutations should have been made");
                result.ShouldContain(x => x.ReplacementNode.IsKind(expectedOutput));
                result.ShouldContain(x => x.ReplacementNode.IsKind(optionalExpectedOutput));
            }
        }
    }
}
