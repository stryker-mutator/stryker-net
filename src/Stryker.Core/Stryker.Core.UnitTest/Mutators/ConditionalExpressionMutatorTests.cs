using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ConditionalExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelComplete()
        {
            var target = new ConditionalExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Complete);
        }

        [Fact]
        public void ShouldMutate_TwoMutations()
        {
            var target = new ConditionalExpressionMutator();
            var check = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(251)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(73))
            );
            var whenTrue = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
            var whenFalse = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0));

            var originalNode = SyntaxFactory.ConditionalExpression(check, whenTrue, whenFalse);

            var result = target.ApplyMutations(originalNode.Condition).ToList();

            result.Count.ShouldBe(2, "Two mutations should have been made");
            Assert.Collection(
                result,
                m1 => Assert.True(m1.ReplacementNode.Kind() is SyntaxKind.TrueLiteralExpression),
                m2 => Assert.True(m2.ReplacementNode.Kind() is SyntaxKind.FalseLiteralExpression)
            );
        }

        [Fact]
        public void ShouldNotMutate_ParentExpression()
        {
            var target = new ConditionalExpressionMutator();
            var check = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(251)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(73))
            );
            var whenTrue = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
            var whenFalse = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0));

            var originalNode = SyntaxFactory.ConditionalExpression(check, whenTrue, whenFalse);

            var result = target.ApplyMutations(originalNode).ToList();

            result.Count.ShouldBe(0, "No mutations should have been made");
        }

        [Fact]
        public void ShouldMutate_DoNotTouchBranches()
        {
            var target = new ConditionalExpressionMutator();
            var check = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(251)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(73))
            );
            var whenTrue = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
            var whenFalse = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0));

            var originalNode = SyntaxFactory.ConditionalExpression(check, whenTrue, whenFalse);

            var result = target.ApplyMutations(originalNode).ToList();

            foreach (var mutation in result)
            {
                Assert.True(mutation.ReplacementNode is ConditionalExpressionSyntax);
                var ces = mutation.ReplacementNode as ConditionalExpressionSyntax;
                Assert.True(ces.WhenTrue.IsEquivalentTo(whenTrue));
                Assert.True(ces.WhenFalse.IsEquivalentTo(whenFalse));
            }
        }
    }
}
