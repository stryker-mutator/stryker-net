using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class PostfixUnaryMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new PostfixUnaryMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression, SyntaxKind.PreIncrementExpression)]
        [InlineData(SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression, SyntaxKind.PreDecrementExpression)]
        public void ShouldMutate(SyntaxKind original, params SyntaxKind[] expectedSyntaxKinds)
        {
            var target = new PostfixUnaryMutator();
            var originalNode = SyntaxFactory.PostfixUnaryExpression(original,
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
    }
}
