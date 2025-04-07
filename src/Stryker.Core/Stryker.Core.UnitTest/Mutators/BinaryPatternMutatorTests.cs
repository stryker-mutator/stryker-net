using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class BinaryPatternMutatorTests : TestBase
{
    [TestMethod]
    [DataRow("and", new[] { SyntaxKind.OrPattern })]
    [DataRow("or", new[] { SyntaxKind.AndPattern })]
    public void ShouldMutateLogicalPattern(string @operator, SyntaxKind[] mutated)
    {
        var target = new BinaryPatternMutator();

        var expression = GenerateWithBinaryPattern(@operator);

        var result = target.ApplyMutations(expression, null).ToList();

        result.ForEach(mutation =>
        {
            mutation.OriginalNode.ShouldBeOfType<BinaryPatternSyntax>();
            mutation.ReplacementNode.ShouldBeOfType<BinaryPatternSyntax>();
            mutation.DisplayName.ShouldBe($"Logical mutation");
        });

        result
            .Select(mutation => (BinaryPatternSyntax)mutation.ReplacementNode)
            .Select(pattern => pattern.Kind())
            .ShouldBe(mutated, true);
    }

    private BinaryPatternSyntax GenerateWithBinaryPattern(string pattern)
    {
        var tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is (1 {pattern} 2);
        }}
    }}
}}");
        var isPatternExpression = tree.GetRoot()
            .DescendantNodes()
            .OfType<BinaryPatternSyntax>()
            .Single();

        return isPatternExpression;
    }
}
