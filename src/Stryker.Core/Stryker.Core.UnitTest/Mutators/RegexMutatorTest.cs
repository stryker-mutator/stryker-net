using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class RegexMutatorTest
    {
        [Fact]
        public void ShouldMutateStringLiteralInRegexConstructor()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
            var target = new RegexMutator();

            var result = target.ApplyMutations(objectCreationExpression);

            var mutation = result.ShouldHaveSingleItem();

            mutation.DisplayName.ShouldBe("Regex mutation");
            var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
            replacement.Token.ValueText.ShouldBe("abc");
        }

        [Fact]
        public void ShouldMutateStringLiteralInRegexConstructorWithFullName()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new System.Text.RegularExpressions.Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
            var target = new RegexMutator();

            var result = target.ApplyMutations(objectCreationExpression);

            var mutation = result.ShouldHaveSingleItem();

            mutation.DisplayName.ShouldBe("Regex mutation");
            var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
            replacement.Token.ValueText.ShouldBe("abc");
        }


        [Fact]
        public void ShouldNotMutateRegexWithoutParameters()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex()") as ObjectCreationExpressionSyntax;
            var target = new RegexMutator();
            var result = target.ApplyMutations(objectCreationExpression);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateStringLiteralInOtherConstructor()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new Other(@\"^abc\")") as ObjectCreationExpressionSyntax;
            var target = new RegexMutator();
            var result = target.ApplyMutations(objectCreationExpression);

            result.ShouldBeEmpty();
        }
    }
}
