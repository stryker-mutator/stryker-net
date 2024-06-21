using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class PostfixUnaryMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new PostfixUnaryMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression)]
        [DataRow(SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new PostfixUnaryMutator();
            var originalNode = SyntaxFactory.PostfixUnaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var result = target.ApplyMutations(originalNode, null).ToList();

            var mutation = result.ShouldHaveSingleItem();
            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.Type.ShouldBe(Mutator.Update);
            mutation.DisplayName.ShouldBe($"{original} to {expected} mutation");
        }
    }
}
