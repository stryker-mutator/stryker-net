using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ConditionStatementMutatorTests
    {
        [Fact]
        public void IfStatement()
        {
            var target = new ConditionStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("Method()"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.First().ReplacementNode.ToString().ShouldBe("!Method()");
        }
        
        [Fact]
        public void IfStatement2()
        {
            var target = new ConditionStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("node.Parent != null"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.First().ReplacementNode.ToString().ShouldBe("node.Parent != null");
        }
        
        [Fact]
        public void WhileStatement()
        {
            var target = new ConditionStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.ParseExpression("Method()"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.First().ReplacementNode.ToString().ShouldBe("!Method()");
        }
        
        [Fact]
        public void ShouldNotMutateIfStatement()
        {
            var target = new ConditionStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("Method(false)"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();

            result.Count.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldNotMutateWhileStatement()
        {
            var target = new ConditionStatementMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.ParseExpression("Method(false)"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();

            result.Count.ShouldBe(0);
        }
    }
}