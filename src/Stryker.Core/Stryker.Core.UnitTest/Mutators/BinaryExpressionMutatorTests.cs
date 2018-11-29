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
        [InlineData(MutatorType.Arithmetic, SyntaxKind.AddExpression, SyntaxKind.SubtractExpression)]
        [InlineData(MutatorType.Arithmetic, SyntaxKind.SubtractExpression, SyntaxKind.AddExpression)]
        [InlineData(MutatorType.Arithmetic, SyntaxKind.MultiplyExpression, SyntaxKind.DivideExpression)]
        [InlineData(MutatorType.Arithmetic, SyntaxKind.DivideExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(MutatorType.Arithmetic, SyntaxKind.ModuloExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)]
        [InlineData(MutatorType.Equality, SyntaxKind.NotEqualsExpression, SyntaxKind.EqualsExpression)]
        [InlineData(MutatorType.Logical, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)]
        [InlineData(MutatorType.Logical, SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression)]
        public void MathMutator_ShouldMutate(MutatorType expectedKind, SyntaxKind input, params SyntaxKind[] expectedOutput)
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(input,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode).ToList();

            if(expectedOutput.Count() == 1)
            {
                // there should be only one mutation
                result.ShouldHaveSingleItem();
            } else
            {
                // there should be two mutations
                result.Count.ShouldBe(2, "Two mutations should have been made");
            }
            int index = 0;
            foreach (var mutation in result)
            {
                mutation.ReplacementNode.IsKind(expectedOutput[index]).ShouldBeTrue();
                mutation.Type.ShouldBe(expectedKind);
                index++;
            }
        }
    }
}
