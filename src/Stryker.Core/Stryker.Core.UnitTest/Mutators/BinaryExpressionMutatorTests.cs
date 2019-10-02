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
        [InlineData(Mutator.Arithmetic, SyntaxKind.AddExpression, SyntaxKind.SubtractExpression)]
        [InlineData(Mutator.Arithmetic, SyntaxKind.SubtractExpression, SyntaxKind.AddExpression)]
        [InlineData(Mutator.Arithmetic, SyntaxKind.MultiplyExpression, SyntaxKind.DivideExpression)]
        [InlineData(Mutator.Arithmetic, SyntaxKind.DivideExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(Mutator.Arithmetic, SyntaxKind.ModuloExpression, SyntaxKind.MultiplyExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)]
        [InlineData(Mutator.Equality, SyntaxKind.NotEqualsExpression, SyntaxKind.EqualsExpression)]
        [InlineData(Mutator.Logical, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)]
        [InlineData(Mutator.Logical, SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression)]
        [InlineData(Mutator.Bitwise, SyntaxKind.BitwiseAndExpression, SyntaxKind.BitwiseOrExpression)]
        [InlineData(Mutator.Bitwise, SyntaxKind.BitwiseOrExpression, SyntaxKind.BitwiseAndExpression)]
        [InlineData(Mutator.Bitwise, SyntaxKind.RightShiftExpression, SyntaxKind.LeftShiftExpression)]
        [InlineData(Mutator.Bitwise, SyntaxKind.LeftShiftExpression, SyntaxKind.RightShiftExpression)]
        public void MathMutator_ShouldMutate(Mutator expectedKind, SyntaxKind input, params SyntaxKind[] expectedOutput)
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
