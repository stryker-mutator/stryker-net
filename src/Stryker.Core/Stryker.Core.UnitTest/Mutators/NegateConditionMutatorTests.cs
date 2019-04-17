using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class NegateConditionMutatorTests
    {
        [Fact]
        public void MutatesIfStatementWithMethodCallWithNoArguments()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("Method()"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.Count.ShouldBe(1);
            result.First().ReplacementNode.ToString().ShouldBe("!Method()");
        }
        
        [Fact]
        public void ShouldNotMutateIfStatementWhenConditionNotInvocationExpression()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("node.Parent != null"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.Count.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldNotMutateIfStatementWithArguments()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.ParseExpression("Method(false)"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();

            result.Count.ShouldBe(0);
        }
        
        [Fact]
        public void MutatesWhileStatementWithMethodCallWithNoArguments()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.ParseExpression("Method()"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.Count.ShouldBe(1);
            result.First().ReplacementNode.ToString().ShouldBe("!Method()");
        }
        
        [Fact]
        public void ShouldNotMutateWhileStatementWithArguments()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.ParseExpression("Method(false)"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();

            result.Count.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldNotMutateWhileStatementWhenConditionNotInvocationExpression()
        {
            var target = new NegateConditionMutator();

            var result = target.ApplyMutations(
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.ParseExpression("node.Parent != null"),
                    SyntaxFactory.EmptyStatement()).Condition).ToList();
            
            result.Count.ShouldBe(0);
        }
    }
}