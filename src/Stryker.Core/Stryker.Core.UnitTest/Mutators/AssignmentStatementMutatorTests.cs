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
            result.ShouldContain(x => x.Type.Equals("AssignmentStatementMutator"));
        }

        [Fact]
        public void ShouldNotMutate()
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
    }
}