using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Shouldly;

namespace Stryker.Core.UnitTest.Mutators
{
    public class CheckedMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.CheckedExpression, "4 + 2", SyntaxKind.AddExpression)]
        public void ShouldMutate(SyntaxKind original, string expression, SyntaxKind expected)
        {
            var target = new CheckedMutator();

            ExpressionSyntax es = SyntaxFactory.ParseExpression(expression);
            var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(original, es)).ToList();

            result.ShouldHaveSingleItem();

            result.First().ReplacementNode.IsKind(expected).ShouldBeTrue();
        }

        [Theory]
        [InlineData(SyntaxKind.UncheckedExpression)]
        public void ShouldNotMutate(SyntaxKind orginal)
        {
            var target = new CheckedMutator();

            ExpressionSyntax es = SyntaxFactory.ParseExpression("4 + 2");
            var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(orginal, es)).ToList();

            result.ShouldBeEmpty();
        }
    }
}
