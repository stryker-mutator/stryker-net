using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Abstractions.Mutators;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class BinaryExpressionMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelBasic()
        {
            var target = new BinaryExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Basic);
        }

        [TestMethod]
        [DataRow(Mutator.Arithmetic, SyntaxKind.AddExpression, SyntaxKind.SubtractExpression)]
        [DataRow(Mutator.Arithmetic, SyntaxKind.SubtractExpression, SyntaxKind.AddExpression)]
        [DataRow(Mutator.Arithmetic, SyntaxKind.MultiplyExpression, SyntaxKind.DivideExpression)]
        [DataRow(Mutator.Arithmetic, SyntaxKind.DivideExpression, SyntaxKind.MultiplyExpression)]
        [DataRow(Mutator.Arithmetic, SyntaxKind.ModuloExpression, SyntaxKind.MultiplyExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)]
        [DataRow(Mutator.Equality, SyntaxKind.NotEqualsExpression, SyntaxKind.EqualsExpression)]
        [DataRow(Mutator.Logical, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)]
        [DataRow(Mutator.Logical, SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression)]
        [DataRow(Mutator.Bitwise, SyntaxKind.BitwiseAndExpression, SyntaxKind.BitwiseOrExpression)]
        [DataRow(Mutator.Bitwise, SyntaxKind.BitwiseOrExpression, SyntaxKind.BitwiseAndExpression)]
        [DataRow(Mutator.Bitwise, SyntaxKind.RightShiftExpression, SyntaxKind.LeftShiftExpression)]
        [DataRow(Mutator.Bitwise, SyntaxKind.LeftShiftExpression, SyntaxKind.RightShiftExpression)]
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
            var index = 0;
            foreach (var mutation in result)
            {
                mutation.ReplacementNode.IsKind(expectedOutput[index]).ShouldBeTrue();
                mutation.Type.ShouldBe(expectedKind);
                mutation.DisplayName.ShouldBe($"{mutation.Type} mutation");
                index++;
            }
        }

        [TestMethod]
        public void ShouldMutate_ExclusiveOr()
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

        [TestMethod]
        public void ShouldNotMutate_StringsLeft()
        {
            var target = new BinaryExpressionMutator();
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }

        [TestMethod]
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
