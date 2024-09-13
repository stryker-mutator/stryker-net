using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class PrefixUnaryMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new PrefixUnaryMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow(SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression)]
        [DataRow(SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression)]
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

        [TestMethod]
        [DataRow(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression)]
        [DataRow(SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression)]
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

        [TestMethod]
        [DataRow(SyntaxKind.BitwiseNotExpression)]
        [DataRow(SyntaxKind.LogicalNotExpression)]
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

        [TestMethod]
        [DataRow(SyntaxKind.AddressOfExpression)]
        [DataRow(SyntaxKind.PointerIndirectionExpression)]
        public void ShouldNotMutate(SyntaxKind original)
        {
            var target = new PrefixUnaryMutator();

            var originalNode = SyntaxFactory.PrefixUnaryExpression(original, SyntaxFactory.IdentifierName("a"));
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
