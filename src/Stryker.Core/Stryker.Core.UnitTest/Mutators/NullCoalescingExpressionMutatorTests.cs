using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class NullCoalescingExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new AssignmentExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Fact]
        public void ShouldKeepTrivia(string originalExpressionString, string[] expectedExpressionStrings)
        {
            // Arrange
            var target = new NullCoalescingExpressionMutator();
            var originalExpressionString = "a ?? b";
            var expectedExpressionStrings = new [] { "a", "b", "b ?? a" };
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as BinaryExpressionSyntax);

            // Assert
            result.Count().ShouldBe(3);

            foreach(var mutant in result)
            {
                expectedExpressionStrings.ShouldContain(mutant.ReplacementNode.ToString());
            }
        }
    }
}
