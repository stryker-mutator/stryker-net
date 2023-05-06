using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class NegateConditionMutatorTests : TestBase
{
    [Fact]
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

    [Theory]
    [InlineData("if (Method()) => return true;")]
    [InlineData("while (Method()) => age++;")]
    [InlineData("(Method()? 1:2);")]
    public void MutatesStatementWithMethodCallWithNoArguments(string method)
    {
        var target = new NegateConditionMutator();

        var node = GenerateExpressions(method);

        var result = target.ApplyMutations(node).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.ReplacementNode.ToString().ShouldBe("!(Method())");
        mutation.DisplayName.ShouldBe("Negate expression");
    }

    [Theory]
    [InlineData("var y = x is object result ? result.ToString() : null;")] // can't mutate inline var declaration
    public void ShouldNotMutateThis(string method)
    {
        var target = new NegateConditionMutator();
        SyntaxTree tree = CSharpSyntaxTree.ParseText(method);


        var expressionSyntax = tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>().Single();
        var result = target.ApplyMutations(expressionSyntax.Condition).ToList();

        result.ShouldBeEmpty();
    }
}
