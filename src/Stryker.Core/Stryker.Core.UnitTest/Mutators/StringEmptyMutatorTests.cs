using System.Linq;
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
            // Arrange
            var node = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                SyntaxFactory.IdentifierName("Empty"));
            var mutator = new StringEmptyMutator();

            // Act
            var result = mutator.ApplyMutations(node).ToList();

            // Assert
            result.ShouldContain(
                m => m.ReplacementNode is LiteralExpressionSyntax &&
                     ((LiteralExpressionSyntax)m.ReplacementNode).Token.ValueText == "Stryker was here!");
        }

        [Fact]
        public void ShouldNotMutateUppercaseString()
        {
            // Arrange
            var node = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("String"),
                SyntaxFactory.IdentifierName("Empty"));
            var mutator = new StringEmptyMutator();

            // Act
            var result = mutator.ApplyMutations(node).ToList();

            // Assert
            result.ShouldNotContain(
                m => m.ReplacementNode is LiteralExpressionSyntax &&
                     ((LiteralExpressionSyntax)m.ReplacementNode).Token.ValueText == "Stryker was here!");
        }

    }
}
