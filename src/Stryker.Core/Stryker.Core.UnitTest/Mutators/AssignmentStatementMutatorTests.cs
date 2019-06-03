using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class AssignmentStatementMutatorTests
    {
        [Theory]
        [InlineData(SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression)]
        [InlineData(SyntaxKind.SubtractAssignmentExpression, SyntaxKind.AddAssignmentExpression)]
        [InlineData(SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.DivideAssignmentExpression)]
        [InlineData(SyntaxKind.DivideAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [InlineData(SyntaxKind.ModuloAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [InlineData(SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression)]
        [InlineData(SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression)]
        [InlineData(SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression)]
        [InlineData(SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.AndAssignmentExpression)]
        public void AssignmentMutator_ShouldMutate(SyntaxKind input, SyntaxKind expectedOutput)
        {
            var target = new AssignmentStatementMutator();
            var originalNode = SyntaxFactory.AssignmentExpression(
                input,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")
            );

            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldHaveSingleItem();
            result.ShouldContain(x => x.ReplacementNode.IsKind(expectedOutput));
            result.ShouldContain(x => x.Type == Mutator.Assignment);
        }

        [Fact]
        public void ShouldNotMutateSimpleAssignment()
        {
            var target = new AssignmentStatementMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")
            );
            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateStringLiteralsLeft()
        {
            var target = new AssignmentStatementMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression),
                SyntaxFactory.IdentifierName("b")
            );
            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldBeEmpty();
        }


        [Fact]
        public void ShouldNotMutateStringLiteralsRight()
        {
            var target = new AssignmentStatementMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.IdentifierName("b"),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression)
            );
            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateStringLiteralsBoth()
        {
            var target = new AssignmentStatementMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression)
            );
            var result = target.ApplyMutations(originalNode).ToList();

            result.ShouldBeEmpty();
        }
    }
}