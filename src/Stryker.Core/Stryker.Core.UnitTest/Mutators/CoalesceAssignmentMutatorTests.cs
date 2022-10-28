using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class CoalesceAssignmentMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new CoalesceAssignmentMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("a ??= b", "a = b")]
        [InlineData("a ??=  b", "a =  b")]
        [InlineData("a  ??= b", "a  = b")]
        [InlineData("a ??=\nb", "a =\nb")]
        [InlineData("a\n??= b", "a\n= b")]
        public void ShouldMutateToSimpleAssignment(string originalExpressionString, string expectedExpressionString)
        {
            // Arrange
            var target = new CoalesceAssignmentMutator();
            var expectedExpressionKind = SyntaxKind.SimpleAssignmentExpression;
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as AssignmentExpressionSyntax);

            // Assert
            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.IsKind(expectedExpressionKind).ShouldBeTrue();
            mutation.ReplacementNode.ToString().ShouldBe(expectedExpressionString);
            mutation.DisplayName.ShouldBe("Coalesce assignment mutation");
        }

        [Fact]
        public void ShouldMutateWithTrivia()
        {
            // Arrange
            var target = new CoalesceAssignmentMutator();
            var originalExpressionKind = SyntaxKind.CoalesceAssignmentExpression;
            var expectedExpressionKind = SyntaxKind.SimpleAssignmentExpression;
            var lhsExpression = SyntaxFactory.IdentifierName("a");
            var rhsExpression = SyntaxFactory.IdentifierName("b");
            var expectedExpression = SyntaxFactory.ParseExpression("a = b");

            // Act
            var result = target.ApplyMutations(SyntaxFactory.AssignmentExpression(originalExpressionKind, lhsExpression, rhsExpression));

            // Assert
            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.IsKind(expectedExpressionKind).ShouldBeTrue();
            mutation.ReplacementNode.ShouldBeSemantically(expectedExpression);
            mutation.DisplayName.ShouldBe("Coalesce assignment mutation");
        }

        [Theory]
        [InlineData(SyntaxKind.AddAssignmentExpression)]
        [InlineData(SyntaxKind.AndAssignmentExpression)]
        [InlineData(SyntaxKind.DivideAssignmentExpression)]
        [InlineData(SyntaxKind.ExclusiveOrAssignmentExpression)]
        [InlineData(SyntaxKind.LeftShiftAssignmentExpression)]
        [InlineData(SyntaxKind.ModuloAssignmentExpression)]
        [InlineData(SyntaxKind.MultiplyAssignmentExpression)]
        [InlineData(SyntaxKind.OrAssignmentExpression)]
        [InlineData(SyntaxKind.RightShiftAssignmentExpression)]
        [InlineData(SyntaxKind.SimpleAssignmentExpression)]
        [InlineData(SyntaxKind.SubtractAssignmentExpression)]
        [InlineData(SyntaxKind.UnsignedRightShiftAssignmentExpression)]
        public void ShouldNotMutate(SyntaxKind originalExpressionKind)
        {
            // Arrange
            var target = new CoalesceAssignmentMutator();
            var lhsExpression = SyntaxFactory.IdentifierName("a");
            var rhsExpression = SyntaxFactory.IdentifierName("b");

            // Act
            var result = target.ApplyMutations(SyntaxFactory.AssignmentExpression(originalExpressionKind, lhsExpression, rhsExpression));

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
