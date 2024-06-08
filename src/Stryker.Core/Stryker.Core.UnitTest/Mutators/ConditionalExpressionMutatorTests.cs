using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ConditionalExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new ConditionalExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Fact]
        public void ShouldMutate_TwoMutations()
        {
            var target = new ConditionalExpressionMutator();
            var source = @"251 == 73 ? 1 : 0";
            var tree = CSharpSyntaxTree.ParseText(source);
            var originalNode = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.Count.ShouldBe(2, "Two mutations should have been made");
            Assert.Collection(
                result,
                m1 => (m1.ReplacementNode is ParenthesizedExpressionSyntax pes && pes.Expression is ConditionalExpressionSyntax ces && ces.Condition.Kind() is SyntaxKind.TrueLiteralExpression).ShouldBeTrue(),
                m2 => (m2.ReplacementNode is ParenthesizedExpressionSyntax pes && pes.Expression is ConditionalExpressionSyntax ces && ces.Condition.Kind() is SyntaxKind.FalseLiteralExpression).ShouldBeTrue()
            );
        }

        [Fact]
        public void ShouldMutate_DoNotTouchBranches()
        {
            var target = new ConditionalExpressionMutator();
            var source = "251 == 73 ? 1 : 0";
            var tree = CSharpSyntaxTree.ParseText(source);
            var originalNode = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();

            var result = target.ApplyMutations(originalNode, null).ToList();

            foreach (var mutation in result)
            {
                var pes = mutation.ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>();
                var ces = pes.Expression.ShouldBeOfType<ConditionalExpressionSyntax>();
                ces.WhenTrue.IsEquivalentTo(originalNode.WhenTrue).ShouldBeTrue();
                ces.WhenFalse.IsEquivalentTo(originalNode.WhenFalse).ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldNotMutateDeclarationPatterns()
        {
            var target = new ConditionalExpressionMutator();
            var source = "var y = x is object result ? result.ToString() : null;";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

            var expressionSyntax = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();
            var result = target.ApplyMutations(expressionSyntax, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
