using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class InterpolatedStringMutatorTests
    {
        private InterpolatedStringExpressionSyntax GetInterpolatedString(string expression)
        {
            return SyntaxFactory.ParseExpression(expression) as InterpolatedStringExpressionSyntax;
        }

        [Theory]
        [InlineData("$\"foo\"")]
        [InlineData("$@\"foo\"")]
        [InlineData("$\"foo {42}\"")]
        public void ShouldMutate(string expression)
        {
            var node = GetInterpolatedString(expression);
            var mutator = new InterpolatedStringMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.ShouldHaveSingleItem()
                .ReplacementNode.ShouldBeOfType<InterpolatedStringExpressionSyntax>()
                .Contents.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("$\"\"")]
        [InlineData("$@\"\"")]
        public void ShouldNotMutateEmptyInterpolatedString(string expression)
        {
            var node = GetInterpolatedString(expression);
            var mutator = new InterpolatedStringMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.ShouldBeEmpty();
        }
    }
}
