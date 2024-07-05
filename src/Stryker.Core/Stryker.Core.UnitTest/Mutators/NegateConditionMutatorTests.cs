using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class NegateConditionMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new NegateConditionMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    /// <summary>
    ///     Generator for different Linq expressions
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

    [TestMethod]
    [DataRow("if (Method()) => return true;")]
    [DataRow("while (Method()) => age++;")]
    public void MutatesStatementWithMethodCallWithNoArguments(string method)
    {
        var target = new NegateConditionMutator();

        var node = GenerateExpressions(method);

        var result = target.ApplyMutations(node, null).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.ReplacementNode.ToString().ShouldBe("!(Method())");
        mutation.DisplayName.ShouldBe("Negate expression");
    }

    [TestMethod]
    [DataRow("var y = x is object result ? result.ToString() : null;")] // can't mutate inline var declaration
    public void ShouldNotMutateThis(string method)
    {
        var target = new NegateConditionMutator();
        SyntaxTree tree = CSharpSyntaxTree.ParseText(method);


        var expressionSyntax = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();
        var result = target.ApplyMutations(expressionSyntax.Condition, null).ToList();

        result.ShouldBeEmpty();
    }
}
