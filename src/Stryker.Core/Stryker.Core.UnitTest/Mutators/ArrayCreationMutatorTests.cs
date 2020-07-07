﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ArrayCreationMutatorTests
    {
        [Fact]
        public void ShouldRemoveValuesFromArrayCreation()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("new int[] { 1, 3 }") as ArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotRemoveValuesFromImplicitArrayCreation()
        {
            var expressionSyntax = SyntaxFactory.ParseExpression("new [] { 1, 3 }") as ImplicitArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(expressionSyntax);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateEmptyInitializer()
        {
            var arrayCreationExpression = SyntaxFactory.ParseExpression("new int[] { }") as ArrayCreationExpressionSyntax;
            var implicitArrayCreationExpression = SyntaxFactory.ParseExpression("new int[] { }") as ArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result1 = target.ApplyMutations(arrayCreationExpression);
            var result2 = target.ApplyMutations(implicitArrayCreationExpression);

            result1.ShouldBeEmpty();
            result2.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldMutateStackallocArrays()
        {
            var stackallocArrayCreationExpression = SyntaxFactory.ParseExpression("stackalloc int[] { 1 }") as StackAllocArrayCreationExpressionSyntax;

            var target = new ArrayCreationMutator();

            var result = target.ApplyMutations(stackallocArrayCreationExpression);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Array initializer mutation");

            var replacement = mutation.ReplacementNode.ShouldBeOfType<StackAllocArrayCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }
    }
}
