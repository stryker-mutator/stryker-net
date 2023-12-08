using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class BinaryExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelBasic()
        {
            var target = new BinaryExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Basic);
        }

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
        public void ShouldMutate(Mutator expectedKind, SyntaxKind input, params SyntaxKind[] expectedOutput)
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(input,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            if (expectedOutput.Count() == 1)
            {
                // there should be only one mutation
                result.ShouldHaveSingleItem();
            }
            else
            {
                // there should be two mutations
                result.Count.ShouldBe(2, "Two mutations should have been made");
            }
            int index = 0;
            foreach (var mutation in result)
            {
                mutation.ReplacementNode.IsKind(expectedOutput[index]).ShouldBeTrue();
                mutation.Type.ShouldBe(expectedKind);
                mutation.DisplayName.ShouldBe($"{mutation.Type} mutation");
                index++;
            }
        }

        [Fact]
        void ShouldMutate_ExclusiveOr()
        {
            var kind = SyntaxKind.ExclusiveOrExpression;
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(kind,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(4)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.Count.ShouldBe(2, "There should be two mutations");
            var logicalMutation = result.SingleOrDefault(x => x.Type == Mutator.Logical);
            logicalMutation.ShouldNotBeNull();
            logicalMutation.ReplacementNode.ShouldNotBeNull();
            logicalMutation.ReplacementNode.IsKind(SyntaxKind.EqualsExpression).ShouldBeTrue();
            logicalMutation.DisplayName.ShouldBe("Logical mutation");

            var integralMutation = result.SingleOrDefault(x => x.Type == Mutator.Bitwise);
            integralMutation.ShouldNotBeNull();
            integralMutation.ReplacementNode.ShouldNotBeNull();
            integralMutation.ReplacementNode.IsKind(SyntaxKind.BitwiseNotExpression).ShouldBeTrue();
            integralMutation.DisplayName.ShouldBe("Bitwise mutation");

            var parenthesizedExpression = integralMutation.ReplacementNode.ChildNodes().SingleOrDefault();
            parenthesizedExpression.ShouldNotBeNull();
            parenthesizedExpression.IsKind(SyntaxKind.ParenthesizedExpression).ShouldBeTrue();

            var exclusiveOrExpression = parenthesizedExpression.ChildNodes().SingleOrDefault();
            exclusiveOrExpression.ShouldNotBeNull();
            exclusiveOrExpression.IsKind(SyntaxKind.ExclusiveOrExpression).ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotMutate_StringsLeft()
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutate_StringsRight()
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
