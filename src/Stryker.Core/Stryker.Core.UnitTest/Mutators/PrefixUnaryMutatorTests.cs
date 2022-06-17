using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class PrefixUnaryMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new PrefixUnaryMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData(SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression)]
        [InlineData(SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression)]
        public void ShouldMutateUnaryTypes(SyntaxKind original, SyntaxKind expected)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldHaveSingleItem();
            var mutation = result.First();
            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.Type.ShouldBe(Mutator.Unary);
        }

        [Theory]
        [InlineData(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression, SyntaxKind.PostIncrementExpression)]
        [InlineData(SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression, SyntaxKind.PostDecrementExpression)]
        public void ShouldMutateUpdateTypes(SyntaxKind original, params SyntaxKind[] expectedSyntaxKinds)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            result.Count.ShouldBe(2, "Two mutations should have been made");

            int index = 0;
            foreach (var mutation in result)
            {
                SyntaxKind expectedSyntaxKind = expectedSyntaxKinds[index];
                mutation.ReplacementNode.IsKind(expectedSyntaxKind).ShouldBeTrue();
                mutation.Type.ShouldBe(Mutator.Update);
                mutation.DisplayName.ShouldBe($"{original} to {expectedSyntaxKind} mutation");
                index++;
            }
        }

        [Theory]
        [InlineData(SyntaxKind.BitwiseNotExpression)]
        [InlineData(SyntaxKind.LogicalNotExpression)]
        public void ShouldMutateAnRemove(SyntaxKind original)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode).ToList();

            var mutation = result.ShouldHaveSingleItem();
            mutation.ReplacementNode.IsKind(SyntaxKind.NumericLiteralExpression).ShouldBeTrue();
            mutation.DisplayName.ShouldBe($"{original} to un-{original} mutation");

            if (original == SyntaxKind.BitwiseNotExpression)
            {
                mutation.Type.ShouldBe(Mutator.Unary);
            }
            else
            {
                mutation.Type.ShouldBe(Mutator.Boolean);
            }
        }

        [Theory]
        [InlineData(SyntaxKind.AddressOfExpression)]
        [InlineData(SyntaxKind.PointerIndirectionExpression)]
        public void ShouldNotMutate(SyntaxKind original)
        {
            var target = new PrefixUnaryMutator();

            var originalNode = SyntaxFactory.PrefixUnaryExpression(original, SyntaxFactory.IdentifierName("a"));
            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldBeEmpty();
        }
    }
}
