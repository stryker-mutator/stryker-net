using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class BooleanMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new BooleanMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression)]
        [InlineData(SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original), null).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.DisplayName.ShouldBe("Boolean mutation");
        }

        [Theory]
        [InlineData(SyntaxKind.NumericLiteralExpression)]
        [InlineData(SyntaxKind.StringLiteralExpression)]
        [InlineData(SyntaxKind.CharacterLiteralExpression)]
        [InlineData(SyntaxKind.NullLiteralExpression)]
        [InlineData(SyntaxKind.DefaultLiteralExpression)]
        public void ShouldNotMutate(SyntaxKind original)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original), null).ToList();

            Assert.Empty(result);
        }
    }
}
