using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class NullCoalescingExpressionMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new NullCoalescingExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Basic);
        }

        [TestMethod]
        public void ShouldMutate()
        {
            // Arrange
            var target = new NullCoalescingExpressionMutator();
            var originalExpressionString = "a ?? b";
            var expectedExpressionStrings = new [] { "a", "b", "b ?? a" };
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as BinaryExpressionSyntax, null);

            // Assert
            result.Count().ShouldBe(3);

            foreach(var mutant in result)
            {
                expectedExpressionStrings.ShouldContain(mutant.ReplacementNode.ToString());
            }
        }

        [TestMethod]
        public void ShouldMutateThrowExpression()
        {
            // Arrange
            var target = new NullCoalescingExpressionMutator();
            var originalExpressionString = "a ?? throw new ArgumentException(nameof(a))";
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as BinaryExpressionSyntax, null);

            // Assert
            var mutant = result.ShouldHaveSingleItem();
            mutant.ReplacementNode.ToString().ShouldBe("a");
        }
    }
}
