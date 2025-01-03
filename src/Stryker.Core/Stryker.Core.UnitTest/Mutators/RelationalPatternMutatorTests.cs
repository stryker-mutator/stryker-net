using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class RelationalPatternMutatorTests : TestBase
{
    [TestMethod]
    [DataRow(">", new[] { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken })]
    [DataRow("<", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken })]
    [DataRow(">=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
    [DataRow("<=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
    public void ShouldMutateRelationalPattern(string @operator, SyntaxKind[] mutated)
    {
        var target = new RelationalPatternMutator();

        var expression = GenerateWithRelationalPattern(@operator).DescendantNodes().OfType<RelationalPatternSyntax>().First();

        var result = target.ApplyMutations(expression, null).ToList();

        result.ForEach(mutation =>
        {
            mutation.OriginalNode.ShouldBeOfType<RelationalPatternSyntax>();
            mutation.ReplacementNode.ShouldBeOfType<RelationalPatternSyntax>();
            mutation.DisplayName.ShouldBe($"Equality mutation");
        });

        result
            .Select(mutation => (RelationalPatternSyntax)mutation.ReplacementNode)
            .Select(pattern => pattern.OperatorToken.Kind())
            .ShouldBe(mutated, true);
    }

    private IsPatternExpressionSyntax GenerateWithRelationalPattern(string @operator)
    {
        var tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is ({@operator} 1);
        }}
    }}
}}");
        var isPatternExpression = tree.GetRoot()
            .DescendantNodes()
            .OfType<IsPatternExpressionSyntax>()
            .Single();

        return isPatternExpression;
    }
}
