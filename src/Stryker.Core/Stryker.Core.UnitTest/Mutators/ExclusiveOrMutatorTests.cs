using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ExclusiveOrMutatorTests
    {
        [Fact]
        void ShouldMutate()
        {
            var kind = SyntaxKind.ExclusiveOrExpression;
            var target = new ExclusiveOrMutator();
            var originalNode = SyntaxFactory.BinaryExpression(kind,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(4)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)));

            var result = target.ApplyMutations(originalNode).ToList();
            
            result.Count.ShouldBe(2, "There should be two mutations");
            var logicalMutation = result.SingleOrDefault(x => x.Type == Mutator.Logical);
            logicalMutation.ShouldNotBeNull();
            logicalMutation.ReplacementNode.ShouldNotBeNull();
            logicalMutation.ReplacementNode.IsKind(SyntaxKind.EqualsExpression).ShouldBeTrue();

            var integralMutation = result.SingleOrDefault(x => x.Type == Mutator.Bitwise);
            integralMutation.ShouldNotBeNull();
            integralMutation.ReplacementNode.ShouldNotBeNull();
            integralMutation.ReplacementNode.IsKind(SyntaxKind.BitwiseNotExpression).ShouldBeTrue();

            var parenthesizedExpression = integralMutation.ReplacementNode.ChildNodes().SingleOrDefault();
            parenthesizedExpression.ShouldNotBeNull();
            parenthesizedExpression.IsKind(SyntaxKind.ParenthesizedExpression).ShouldBeTrue();

            var exclusiveOrExpression = parenthesizedExpression.ChildNodes().SingleOrDefault();
            exclusiveOrExpression.ShouldNotBeNull();
            exclusiveOrExpression.IsKind(SyntaxKind.ExclusiveOrExpression).ShouldBeTrue();

        }
    }
}
