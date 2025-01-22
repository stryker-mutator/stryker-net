using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class StringMethodToConstantMutatorTests : TestBase
{
    [TestMethod]
    [DataRow("Trim")]
    [DataRow("Substring")]
    public void ShouldMutateReplaceWithEmptyString(string methodName)
    {
        var expression = $"testString.{methodName}()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(expression);
        var target = new StringMethodToConstantMutator();
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
        var target = new StringMethodToConstantMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.StringMethod);
        mutation.DisplayName.ShouldBe($"String Method Mutation (Replace {methodName}() with '\\0' char)");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        syntax.Token.ValueText.ShouldBe(char.MinValue.ToString());
    }

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
}
