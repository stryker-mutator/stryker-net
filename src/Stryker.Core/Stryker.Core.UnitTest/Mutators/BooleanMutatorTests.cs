using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Abstractions.Mutators;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Abstractions.UnitTest.Mutators
{
    [TestClass]
    public class BooleanMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new BooleanMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression)]
        [DataRow(SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original), null).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
            mutation.DisplayName.ShouldBe("Boolean mutation");
        }

        [TestMethod]
        [DataRow(SyntaxKind.NumericLiteralExpression)]
        [DataRow(SyntaxKind.StringLiteralExpression)]
        [DataRow(SyntaxKind.CharacterLiteralExpression)]
        [DataRow(SyntaxKind.NullLiteralExpression)]
        [DataRow(SyntaxKind.DefaultLiteralExpression)]
        public void ShouldNotMutate(SyntaxKind original)
        {
            var target = new BooleanMutator();

            var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original), null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
