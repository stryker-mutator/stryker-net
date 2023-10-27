using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class InterpolatedStringMutatorTests : TestBase
    {
        private InterpolatedStringExpressionSyntax GetInterpolatedString(string expression)
        {
            return SyntaxFactory.ParseExpression(expression) as InterpolatedStringExpressionSyntax;
        }

        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new InterpolatedStringMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("$\"foo\"")]
        [InlineData("$@\"foo\"")]
        [InlineData("$\"foo {42}\"")]
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

        [Theory]
        [InlineData("$\"\"")]
        [InlineData("$@\"\"")]
        public void ShouldNotMutateEmptyInterpolatedString(string expression)
        {
            var node = GetInterpolatedString(expression);
            var mutator = new InterpolatedStringMutator();

            var result = mutator.ApplyMutations(node, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
