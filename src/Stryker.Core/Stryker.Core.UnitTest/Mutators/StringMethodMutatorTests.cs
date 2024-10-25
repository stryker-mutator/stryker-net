using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class StringMethodMutatorTests : TestBase
{
    private static (SemanticModel semanticModel, InvocationExpressionSyntax expression)
        CreateSemanticModelFromExpression(string input)
    {
        // Parse the code into a syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(
            $$"""
              using System.Linq;

              class TestClass {
              private string testString = "test";
              private char c = 't';

              void TestMethod() {
                      {{input}} ";
                  }
              }
              """);

        // Create a compilation that contains the syntax tree
        var compilation = CSharpCompilation.Create("TestAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);

        // Get the semantic model from the compilation
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().First();

        return (semanticModel, expression);
    }

    [TestMethod]
    [DataRow("testString.EndsWith(c)", "StartsWith",
        "String Method Mutation (Replace EndsWith() with StartsWith())")]
    [DataRow("testString.StartsWith(c)", "EndsWith",
        "String Method Mutation (Replace StartsWith() with EndsWith())")]
    [DataRow("testString.TrimStart()", "TrimEnd", "String Method Mutation (Replace TrimStart() with TrimEnd())")]
    [DataRow("testString.TrimEnd()", "TrimStart", "String Method Mutation (Replace TrimEnd() with TrimStart())")]
    [DataRow("testString.ToUpper()", "ToLower", "String Method Mutation (Replace ToUpper() with ToLower())")]
    [DataRow("testString.ToLower()", "ToUpper", "String Method Mutation (Replace ToLower() with ToUpper())")]
    [DataRow("testString.ToUpperInvariant()", "ToLowerInvariant",
        "String Method Mutation (Replace ToUpperInvariant() with ToLowerInvariant())")]
    [DataRow("testString.ToLowerInvariant()", "ToUpperInvariant",
        "String Method Mutation (Replace ToLowerInvariant() with ToUpperInvariant())")]
    [DataRow("testString.PadLeft(10)", "PadRight", "String Method Mutation (Replace PadLeft() with PadRight())")]
    [DataRow("testString.PadRight(10)", "PadLeft", "String Method Mutation (Replace PadRight() with PadLeft())")]
    [DataRow("testString.LastIndexOf(c)", "IndexOf", "String Method Mutation (Replace LastIndexOf() with IndexOf())")]
    [DataRow("testString.IndexOf(c)", "LastIndexOf", "String Method Mutation (Replace IndexOf() with LastIndexOf())")]
    public void ShouldMutateStringMethods(string expression, string mutatedMethod, string expectedDisplayName)
    {
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.Type.ShouldBe(Mutator.StringMethod);
        mutation.DisplayName.ShouldBe(expectedDisplayName);

        var access = mutation.ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>();
        access.Name.Identifier.Text.ShouldBe(mutatedMethod);
    }

    [TestMethod]
    [DataRow("Trim")]
    [DataRow("Substring")]
    public void ShouldMutateReplaceWithEmptyString(string methodName)
    {
        var expression = $"testString.{methodName}()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.StringMethod);
        mutation.DisplayName.ShouldBe($"String Method Mutation (Replace {methodName}() with Empty String)");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        syntax.Token.ValueText.ShouldBe(string.Empty);
    }

    [TestMethod]
    [DataRow("ElementAt")]
    [DataRow("ElementAtOrDefault")]
    public void ShouldMutateReplaceWithChar(string methodName)
    {
        var expression = $"testString.{methodName}()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.StringMethod);
        mutation.DisplayName.ShouldBe($"String Method Mutation (Replace {methodName}() with '\\0' char)");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        syntax.Token.ValueText.ShouldBe(char.MinValue.ToString());
    }

    [TestMethod]
    public void ShouldNotMutateWhenNotAString()
    {
        var expression = (InvocationExpressionSyntax)
            SyntaxFactory.ParseExpression("Enumerable.Max(new[] { 1, 2, 3 })");
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expression, null).ToList();

        result.ShouldBeEmpty();
    }
}
