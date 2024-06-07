using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

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

    [Theory]
    [InlineData("testString.EndsWith(c)", "StartsWith",
        "String Method Mutation (Replace EndsWith() with StartsWith())")]
    [InlineData("testString.StartsWith(c)", "EndsWith",
        "String Method Mutation (Replace StartsWith() with EndsWith())")]
    [InlineData("testString.TrimStart()", "TrimEnd", "String Method Mutation (Replace TrimStart() with TrimEnd())")]
    [InlineData("testString.TrimEnd()", "TrimStart", "String Method Mutation (Replace TrimEnd() with TrimStart())")]
    [InlineData("testString.ToUpper()", "ToLower", "String Method Mutation (Replace ToUpper() with ToLower())")]
    [InlineData("testString.ToLower()", "ToUpper", "String Method Mutation (Replace ToLower() with ToUpper())")]
    [InlineData("testString.ToUpperInvariant()", "ToLowerInvariant",
        "String Method Mutation (Replace ToUpperInvariant() with ToLowerInvariant())")]
    [InlineData("testString.ToLowerInvariant()", "ToUpperInvariant",
        "String Method Mutation (Replace ToLowerInvariant() with ToUpperInvariant())")]
    [InlineData("testString.PadLeft(10)", "PadRight", "String Method Mutation (Replace PadLeft() with PadRight())")]
    [InlineData("testString.PadRight(10)", "PadLeft", "String Method Mutation (Replace PadRight() with PadLeft())")]
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

    [Theory]
    [InlineData("Trim")]
    [InlineData("Substring")]
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

    [Theory]
    [InlineData("ElementAt")]
    [InlineData("ElementAtOrDefault")]
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

    [Fact]
    public void ShouldNotMutateWhenNotAString()
    {
        var expression = (InvocationExpressionSyntax)
            SyntaxFactory.ParseExpression("Enumerable.Max(new[] { 1, 2, 3 })");
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expression, null).ToList();

        result.ShouldBeEmpty();
    }
}
