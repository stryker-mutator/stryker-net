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

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldHaveSingleItem();
            var mutation = result.First();
            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.Type.ShouldBe(Mutator.Unary);
        }

        [Theory]
        [InlineData(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression)]
        [InlineData(SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression)]
        public void ShouldMutateUpdateTypes(SyntaxKind original, SyntaxKind expected)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldHaveSingleItem();
            var mutation = result.First();
            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.Type.ShouldBe(Mutator.Update);
        }

        [Theory]
        [InlineData(SyntaxKind.BitwiseNotExpression)]
        [InlineData(SyntaxKind.LogicalNotExpression)]
        public void ShouldMutateAnRemove(SyntaxKind original)
        {
            var target = new PrefixUnaryMutator();
            var originalNode = SyntaxFactory.PrefixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode, null).ToList();

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
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
