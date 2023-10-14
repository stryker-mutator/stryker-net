using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ArrayCreationMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new ArrayCreationMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("new int[] { 1, 3 }")]
        [InlineData("new int[2] { 1, 3 }")]
        public void ShouldClearInitializerOfArrayCreation(string expression)
        {
            var expressionSyntax = SyntaxFactory.ParseExpression(expression) as ArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("stackalloc int[] { 1, 3 }")]
        [InlineData("stackalloc int[2] { 1, 3 }")]
        public void ShouldClearInitializerOfStackAllocArrayCreation(string expression)
        {
            var stackallocArrayCreationExpression = SyntaxFactory.ParseExpression(expression) as StackAllocArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(stackallocArrayCreationExpression);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");

            var replacement = mutation.ReplacementNode.ShouldBeOfType<StackAllocArrayCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldMutateImplicitArrayCreationToDefault()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("new [] { 1, 3 }") as ImplicitArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");
            var literalExpression = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
            literalExpression.Token.Kind().ShouldBe(SyntaxKind.DefaultKeyword);
        }

        [Fact]
        public void ShouldMutateImplicitStackAllocArrayCreationToDefault()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("stackalloc [] { 1, 3 }") as ImplicitStackAllocArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");
            var literalExpression = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
            literalExpression.Token.Kind().ShouldBe(SyntaxKind.DefaultKeyword);
        }

        [Theory]
        [InlineData("new int[] { }")]
        [InlineData("stackalloc int[] { }")]
        [InlineData("new [] { }")]
        [InlineData("stackalloc [] { }")]
        public void ShouldNotMutateEmptyInitializer(string expression)
        {
            var expressionSyntax = SyntaxFactory.ParseExpression(expression);

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            result.ShouldBeEmpty();
        }
    }
}
