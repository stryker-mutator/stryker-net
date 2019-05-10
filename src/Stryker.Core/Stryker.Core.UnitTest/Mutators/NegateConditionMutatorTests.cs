using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class NegateConditionMutatorTests
    {

        /// <summary>
        ///     Generator for different Linqexpressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private InvocationExpressionSyntax GenerateExpressions(string expression)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;
 
namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            {expression}
        }}
    }}
}}");
            var invocationExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Single();

            return invocationExpression;
        }

        [Theory]
        [InlineData("if (Method()) => return true;")]
        [InlineData("while (Method()) => age++;")]
        public void MutatesStatementWithMethodCallWithNoArguments(string method)
        {
            var target = new NegateConditionMutator();

            var node = GenerateExpressions(method);

            var result = target.ApplyMutations(node).ToList();
            
            result.Count.ShouldBe(1);
            result.First().ReplacementNode.ToString().ShouldBe("!Method()");
        }

        [Theory]
        [InlineData("if (Method(node.Parent != null)) => return true;")]
        [InlineData("while (Method(false)) => return true;")]
        public void ShouldNotMutateStatementWithArguments(string method)
        {
            var target = new NegateConditionMutator();

            var node = GenerateExpressions(method);

            var result = target.ApplyMutations(node).ToList();

            result.Count.ShouldBe(0);
        }
    }
}