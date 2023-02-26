using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class StringIsNullOrMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new StringIsNullOrMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("string.Empty")]
        [InlineData("args[0].Substring(1)")]
        public void ShouldMutateIsNullOrEmpty(string argument)
        {
            var expression = GenerateCall(nameof(string.IsNullOrEmpty), argument);
            var target = new StringIsNullOrMutator();
            var mutated = target.ApplyMutations(expression).ToList();

            mutated.Count.ShouldBe(2);
            mutated.ForEach(mutation =>
            {
                mutation.OriginalNode.ShouldBe(expression);
                mutation.DisplayName.ShouldBe("String mutation");
                mutation.Type.ShouldBe(Mutator.String);

                var binaryExpression = mutation.ReplacementNode.ShouldBeOfType<BinaryExpressionSyntax>();

                binaryExpression.Kind().ShouldBe(SyntaxKind.NotEqualsExpression);
                binaryExpression.Left.ToString().ShouldBe(expression.ArgumentList.Arguments[0].Expression.ToString());
            });

            var nullMutation = (BinaryExpressionSyntax)mutated[0].ReplacementNode;
            var nullLiteral = nullMutation.Right.ShouldBeOfType<LiteralExpressionSyntax>();

            nullLiteral.Kind().ShouldBe(SyntaxKind.NullLiteralExpression);

            var emptyMutation = (BinaryExpressionSyntax)mutated[1].ReplacementNode;
            var emptyLiteral = emptyMutation.Right.ShouldBeOfType<LiteralExpressionSyntax>();

            emptyLiteral.Kind().ShouldBe(SyntaxKind.StringLiteralExpression);
            emptyLiteral.Token.ToString().ShouldBe(@"""""");
        }

        [Theory]
        [InlineData("x")]
        [InlineData("string.Empty")]
        [InlineData("args[0].Substring(1)")]
        public void ShouldMutateIsNullOrWhiteSpace(string argument)
        {
            var expression = GenerateCall(nameof(string.IsNullOrWhiteSpace), argument);
            var target = new StringIsNullOrMutator();
            var mutated = target.ApplyMutations(expression).ToList();

            mutated.Count.ShouldBe(3);
            mutated.ForEach(mutation =>
            {
                mutation.OriginalNode.ShouldBe(expression);
                mutation.DisplayName.ShouldBe("String mutation");
                mutation.Type.ShouldBe(Mutator.String);

                var binaryExpression = mutation.ReplacementNode.ShouldBeOfType<BinaryExpressionSyntax>();

                binaryExpression.Kind().ShouldBe(SyntaxKind.NotEqualsExpression);
                binaryExpression.Left.ToString().ShouldStartWith(expression.ArgumentList.Arguments[0].Expression.ToString());
            });

            var nullMutation = (BinaryExpressionSyntax)mutated[0].ReplacementNode;
            var nullLiteral = nullMutation.Right.ShouldBeOfType<LiteralExpressionSyntax>();

            nullLiteral.Kind().ShouldBe(SyntaxKind.NullLiteralExpression);

            var emptyMutation = (BinaryExpressionSyntax)mutated[1].ReplacementNode;
            var emptyLiteral = emptyMutation.Right.ShouldBeOfType<LiteralExpressionSyntax>();

            emptyLiteral.Kind().ShouldBe(SyntaxKind.StringLiteralExpression);
            emptyLiteral.Token.ToString().ShouldBe(@"""""");

            var whiteSpaceMutation = (BinaryExpressionSyntax)mutated[2].ReplacementNode;
            var whiteSpaceLiteral = whiteSpaceMutation.Right.ShouldBeOfType<LiteralExpressionSyntax>();

            whiteSpaceMutation.Left.ToString().ShouldBe(expression.ArgumentList.Arguments[0].Expression.ToString() + ".Trim().Length");
            whiteSpaceLiteral.Kind().ShouldBe(SyntaxKind.NumericLiteralExpression);
            whiteSpaceLiteral.Token.ToString().ShouldBe("0");
        }

        [Theory]
        [InlineData("IsNormalized")]
        [InlineData("Test")]
        [InlineData("IsNotNullOrNotEmpty")]
        public void ShouldNotMutateOtherMethods(string method)
        {
            var expression = GenerateCall(method, "string.Empty");
            var target = new StringIsNullOrMutator();
            var mutated = target.ApplyMutations(expression).ToList();

            mutated.ShouldBeEmpty();
        }

        private InvocationExpressionSyntax GenerateCall(string method, string argument)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = string.{method}({argument});
        }}
    }}
}}");
            var invocationExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .First();

            return invocationExpression;
        }
    }
}
