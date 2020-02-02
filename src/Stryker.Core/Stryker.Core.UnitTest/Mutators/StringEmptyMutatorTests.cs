﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class StringEmptyMutatorTests
    {
        [Fact]
        public void ShouldMutateLowercaseString()
        {
            var node = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                SyntaxFactory.IdentifierName("Empty"));
            var mutator = new StringEmptyMutator();

            var result = mutator.ApplyMutations(node).ToList();

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("String mutation");
            var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
            replacement.Token.ValueText.ShouldBe("Stryker was here!");
        }

        [Fact]
        public void ShouldNotMutateUppercaseString()
        {
            var node = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("String"),
                SyntaxFactory.IdentifierName("Empty"));
            var mutator = new StringEmptyMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.ShouldBeEmpty();
        }

    }
}
