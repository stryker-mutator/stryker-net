using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class IfStatementMutatorTests
    {
        [Fact]
        public void ShouldMutateCondition()
        {
            var target = new IfStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("new[]{1}.Contains(1)"),
                    SyntaxFactory.EmptyStatement())).ToList();

            Assert.Single(result);

            Assert.False(result.First().OriginalNode.IsEquivalentTo(result.First().ReplacementNode));
        }
        
        [Fact]
        public void ShouldNotMutateStatement()
        {
            var target = new IfStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("new[]{1}.Contains(1)"),
                    SyntaxFactory.EmptyStatement())).ToList();

            Assert.Single(result);

            var originalNode = result.First().OriginalNode as IfStatementSyntax;
            var replacedNode = result.First().ReplacementNode as IfStatementSyntax;

            Assert.True(originalNode.Statement.IsEquivalentTo(replacedNode.Statement));
        }
    }
}