using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.UnitTest.Mutants;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class AddMutatorTests
    {
        /// <summary>
        /// Test if the AddMutator mutates an AddExpression to a SubtractExpression
        /// </summary>
        [Theory]
        [InlineData(SyntaxKind.AddExpression, SyntaxKind.SubtractExpression)]
        public void ShouldMutate(SyntaxKind original, SyntaxKind expected)
        {
            var target = new AddMutator();
            var originalNode = SyntaxFactory.BinaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Single(result);
            Assert.True(result.First().ReplacementNode.IsKind(expected));
        }

        /// <summary>
        /// Test if the AddMutator does not mutate other expressions
        /// </summary>
        [Theory]
        [InlineData(SyntaxKind.SubtractExpression)]
        [InlineData(SyntaxKind.DivideExpression)]
        [InlineData(SyntaxKind.MultiplyExpression)]
        [InlineData(SyntaxKind.ModuloExpression)]
        [InlineData(SyntaxKind.GreaterThanExpression)]
        public void ShouldNotMutate(SyntaxKind original)
        {
            var target = new AddMutator();
            var originalNode = SyntaxFactory.BinaryExpression(original,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var result = target.ApplyMutations(originalNode).ToList();

            Assert.Empty(result);
        }
    }
}
