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
        [Fact]
        public void ShouldBeMutationlevelStandard()
        {
            var target = new AssignmentExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData(SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression)]
        [InlineData(SyntaxKind.SubtractAssignmentExpression, SyntaxKind.AddAssignmentExpression)]
        [InlineData(SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.DivideAssignmentExpression)]
        [InlineData(SyntaxKind.DivideAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [InlineData(SyntaxKind.ModuloAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [InlineData(SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression)]
        [InlineData(SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression)]
        [InlineData(SyntaxKind.AndAssignmentExpression, SyntaxKind.OrAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression)]
        [InlineData(SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression)]
        [InlineData(SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression)]
        public void AssignmentMutator_ShouldMutate(SyntaxKind input, SyntaxKind expectedOutput, SyntaxKind? additionalOutput = null)
        {
            var target = new AssignmentExpressionMutator();
            var originalNode = SyntaxFactory.AssignmentExpression(
                input,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")
            );

            var result = target.ApplyMutations(originalNode).ToList();

            if (additionalOutput.HasValue && additionalOutput.Value is var additionalExpectedOutput)
            {
                result.Count.ShouldBe(2);
                result.First().ReplacementNode.IsKind(expectedOutput).ShouldBeTrue();
                result.Last().ReplacementNode.IsKind(additionalExpectedOutput).ShouldBeTrue();
            }
            else
            {
                var mutation = result.ShouldHaveSingleItem();
                mutation.ReplacementNode.IsKind(expectedOutput).ShouldBeTrue();
            }
            foreach(var mutation in result)
            {
                mutation.Type.ShouldBe(Mutator.Assignment);
                mutation.DisplayName.ShouldBe($"{input} to {mutation.ReplacementNode.Kind()} mutation");
            }
        }

        [Fact]
        public void ShouldNotMutateSimpleAssignment()
        {
            var target = new AssignmentExpressionMutator();

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
            var target = new AssignmentExpressionMutator();

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
            var target = new AssignmentExpressionMutator();

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
            var target = new AssignmentExpressionMutator();

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
