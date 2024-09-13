using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class InterpolatedStringMutatorTests : TestBase
    {
        private InterpolatedStringExpressionSyntax GetInterpolatedString(string expression)
        {
            return SyntaxFactory.ParseExpression(expression) as InterpolatedStringExpressionSyntax;
        }

        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new InterpolatedStringMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow("$\"foo\"")]
        [DataRow("$@\"foo\"")]
        [DataRow("$\"foo {42}\"")]
        public void ShouldMutate(string expression)
        {
            var node = GetInterpolatedString(expression);
            var mutator = new InterpolatedStringMutator();

            var result = mutator.ApplyMutations(node, null).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ShouldBeOfType<InterpolatedStringExpressionSyntax>()
                .Contents.ShouldBeEmpty();
            mutation.DisplayName.ShouldBe("String mutation");
        }

        [TestMethod]
        [DataRow("$\"\"")]
        [DataRow("$@\"\"")]
        public void ShouldNotMutateEmptyInterpolatedString(string expression)
        {
            var node = GetInterpolatedString(expression);
            var mutator = new InterpolatedStringMutator();

            var result = mutator.ApplyMutations(node, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
