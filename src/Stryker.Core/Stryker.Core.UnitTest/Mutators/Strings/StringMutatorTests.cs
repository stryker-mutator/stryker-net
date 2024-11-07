using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Abstractions.Mutators;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators.Strings;

[TestClass]
public class StringMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new StringMutator();
        target.OtherMutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    [DataRow("", "Stryker was here!")]
    [DataRow("foo", "")]
    public void ShouldMutate(string original, string expected)
    {
        var node = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(original));
        var mutator = new StringMutator();

        var result = mutator.ApplyMutations(node, null, MutationLevel.Standard).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
            .Token.Value.ShouldBe(expected);
        mutation.DisplayName.ShouldBe("String mutation");
    }

    [TestMethod]
    public void ShouldNotMutateOnRegexExpression()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new Regex(\"myregex\")");
        var literalExpression = expressionSyntax.DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateOnFullyDefinedRegexExpression()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new System.Text.RegularExpressions.Regex(\"myregex\")");
        var literalExpression = expressionSyntax.DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateOnRegularExpressionInClass()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"using System.Text.RegularExpressions;
namespace Stryker.Core.UnitTest.Mutators
{
    public class Test {
        public Regex GetRegex(){
            return new Regex(""myregex"");
        }
    }
}
");
        var literalExpression = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateOnGuidExpression()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new Guid(\"00000-0000\")");
        var literalExpression = expressionSyntax.DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateOnFullyDefinedGuidExpression()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new System.Guid(\"00000-0000\")");
        var literalExpression = expressionSyntax.DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateOnGuidInClass()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;
namespace Stryker.Core.UnitTest.Mutators
{
    public class Test {
        public Guid GetGuid(){
            return new Guid(""00000-0000"");
        }
    }
}
");
        var literalExpression = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();
        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard).ToList();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow(@"""""u8", @"""Stryker was here!""u8")]
    [DataRow(@"""foo""u8", @"""""u8")]
    public void ShouldMutateUtf8StringLiteral(string original, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText($"var test = {original};");

        var literalExpression = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();
        var mutator = new StringMutator();

        var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard);

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
                .Token.Text.ShouldBe(expected);
        mutation.DisplayName.ShouldBe("String mutation");
    }

    [TestMethod]
    [DataRow(@"""""u8 + """"u8")]
    [DataRow(@"""foo""u8 + """"u8")]
    [DataRow(@"""""u8 + ""foo""u8")]
    [DataRow(@"""foo""u8 + ""foo""u8 + ""foo""u8")]
    public void ShouldNotMutateConcatenatedUtf8StringLiteral(string original)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText($"var test = {original};");

        var mutator = new StringMutator();
        foreach (var literalExpression in syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>())
        {
            var result = mutator.ApplyMutations(literalExpression, null, MutationLevel.Standard);
            result.ShouldBeEmpty();
        }
    }
}
