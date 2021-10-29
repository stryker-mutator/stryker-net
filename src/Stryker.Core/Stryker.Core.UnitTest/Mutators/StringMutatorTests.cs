using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class StringMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new StringMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("", "Stryker was here!")]
        [InlineData("foo", "")]
        public void ShouldMutate(string original, string expected)
        {
            var node = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(original));
            var mutator = new StringMutator();

            var result = mutator.ApplyMutations(node).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
                .Token.Value.ShouldBe(expected);
            mutation.DisplayName.ShouldBe("String mutation");
        }

        [Fact]
        public void ShouldNotMutateOnRegexExpression()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("new Regex(\"myregex\")");
            var literalExpression = expressionSyntax.DescendantNodes().OfType<LiteralExpressionSyntax>().First();
            var mutator = new StringMutator();
            var result = mutator.ApplyMutations(literalExpression).ToList();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateOnRegularExpressionInClass()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System.Text.RegularExpressions;
namespace Stryker.Core.UnitTest.Mutators
{
    public class Test {
        public Regex GetRegex(){
            return new Regex(""myregex"");
        }
    }
}
");
            var literalExpression = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();
            var mutator = new StringMutator();
            var result = mutator.ApplyMutations(literalExpression).ToList();

            result.ShouldBeEmpty();
        }
    }
}
