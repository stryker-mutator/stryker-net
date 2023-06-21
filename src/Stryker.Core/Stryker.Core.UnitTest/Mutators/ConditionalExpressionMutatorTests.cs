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

            var result = target.ApplyMutations(originalNode).ToList();

            result.Count.ShouldBe(2, "Two mutations should have been made");
            Assert.Collection(
                result,
                m1 => Assert.True(m1.ReplacementNode is ParenthesizedExpressionSyntax pes && pes.Expression is ConditionalExpressionSyntax ces && ces.Condition.Kind() is SyntaxKind.TrueLiteralExpression),
                m2 => Assert.True(m2.ReplacementNode is ParenthesizedExpressionSyntax pes && pes.Expression is ConditionalExpressionSyntax ces && ces.Condition.Kind() is SyntaxKind.FalseLiteralExpression)
            );
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
                Assert.True(mutation.ReplacementNode is ParenthesizedExpressionSyntax);
                var pes = mutation.ReplacementNode as ParenthesizedExpressionSyntax;
                Assert.True(pes.Expression is ConditionalExpressionSyntax);
                var ces = pes.Expression as ConditionalExpressionSyntax;
                Assert.True(ces.WhenTrue.IsEquivalentTo(whenTrue));
                Assert.True(ces.WhenFalse.IsEquivalentTo(whenFalse));
            }
        }
    }
}
