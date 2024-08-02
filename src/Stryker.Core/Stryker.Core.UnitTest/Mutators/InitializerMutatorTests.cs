using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Configuration.Mutators;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Mutators
{
    [TestClass]
    public class InitializerMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new InitializerMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        public void ShouldRemoveValuesFromArrayInitializer()
        {
            var initializerExpression = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression,
                SyntaxFactory.SeparatedList(new List<ExpressionSyntax> {
                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(5))
                }));
            var target = new InitializerMutator();

            var result = target.ApplyMutations(initializerExpression, null);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");

            var replacement = mutation.ReplacementNode.ShouldBeOfType<InitializerExpressionSyntax>();
            replacement.Expressions.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateEmptyInitializer()
        {
            var emptyInitializerExpression = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression,
                SyntaxFactory.SeparatedList<ExpressionSyntax>());
            var target = new InitializerMutator();

            var result = target.ApplyMutations(emptyInitializerExpression, null);

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateStackallocArrayCreationExpressionSyntax()
        {
            var arrayCreationExpression = SyntaxFactory.ParseExpression("stackalloc int[] { 0 }") as StackAllocArrayCreationExpressionSyntax;

            var target = new InitializerMutator();

            var result = target.ApplyMutations(arrayCreationExpression.Initializer, null);

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateArrayCreationExpressionSyntax()
        {
            var arrayCreationExpression = SyntaxFactory.ParseExpression("new int[] { 0 }") as ArrayCreationExpressionSyntax;

            var target = new InitializerMutator();

            var result = target.ApplyMutations(arrayCreationExpression.Initializer, null);

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateImplicitArrayCreationExpressionSyntax()
        {
            var arrayCreationExpression = SyntaxFactory.ParseExpression("new [] { 0 }") as ImplicitArrayCreationExpressionSyntax;

            var target = new InitializerMutator();

            var result = target.ApplyMutations(arrayCreationExpression.Initializer, null);

            result.ShouldBeEmpty();
        }
    }
}
