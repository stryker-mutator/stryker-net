using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class AssignmentStatementMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new AssignmentExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow(SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression)]
        [DataRow(SyntaxKind.SubtractAssignmentExpression, SyntaxKind.AddAssignmentExpression)]
        [DataRow(SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.DivideAssignmentExpression)]
        [DataRow(SyntaxKind.DivideAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [DataRow(SyntaxKind.ModuloAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression)]
        [DataRow(SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression)]
        [DataRow(SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression)]
        [DataRow(SyntaxKind.AndAssignmentExpression, SyntaxKind.OrAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression)]
        [DataRow(SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression)]
        [DataRow(SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression)]
        [DataRow(SyntaxKind.CoalesceAssignmentExpression, SyntaxKind.SimpleAssignmentExpression)]
        public void AssignmentMutator_ShouldMutate(SyntaxKind input, SyntaxKind expectedOutput, SyntaxKind? additionalOutput = null)
        {
            var target = new AssignmentExpressionMutator();
            var originalNode = SyntaxFactory.AssignmentExpression(
                input,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")
            );

            var result = target.ApplyMutations(originalNode, null).ToList();

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

            foreach (var mutation in result)
            {
                mutation.Type.ShouldBe(Mutator.Assignment);
                mutation.DisplayName.ShouldBe($"{input} to {mutation.ReplacementNode.Kind()} mutation");
            }
        }

        [TestMethod]
        [DataRow("a += b", "a -= b")]
        [DataRow("a +=  b", "a -=  b")]
        [DataRow("a  += b", "a  -= b")]
        [DataRow("a +=\nb", "a -=\nb")]
        [DataRow("a\n+= b", "a\n-= b")]
        public void ShouldKeepTrivia(string originalExpressionString, string expectedExpressionString)
        {
            // Arrange
            var target = new AssignmentExpressionMutator();
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as AssignmentExpressionSyntax, null);

            // Assert
            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ToString().ShouldBe(expectedExpressionString);
        }

        [TestMethod]
        public void ShouldNotMutateSimpleAssignment()
        {
            var target = new AssignmentExpressionMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")
            );
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateStringLiteralsLeft()
        {
            var target = new AssignmentExpressionMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression),
                SyntaxFactory.IdentifierName("b")
            );
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateStringLiteralsRight()
        {
            var target = new AssignmentExpressionMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.IdentifierName("b"),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression)
            );
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNotMutateStringLiteralsBoth()
        {
            var target = new AssignmentExpressionMutator();

            var originalNode = SyntaxFactory.AssignmentExpression(
                SyntaxKind.AddAssignmentExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression)
            );
            var result = target.ApplyMutations(originalNode, null).ToList();

            result.ShouldBeEmpty();
        }
    }
}
