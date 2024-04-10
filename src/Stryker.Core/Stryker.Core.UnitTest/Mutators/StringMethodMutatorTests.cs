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
            """
                         using System.Linq;

                         class TestClass {
                            private string testString = "test";
                            private char c = 't';

                            void TestMethod() {
            """ + " " + input + "; } }");

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

    [Fact]
    public void ShouldMutateEndsWith()
    {
        const string Expression = "testString.EndsWith(x)";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace EndsWith() with StartsWith())");

        var invocationExpression =
            mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            invocationExpression.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("StartsWith");
    }

    [Fact]
    public void ShouldMutateStartsWith()
    {
        const string Expression = "testString.StartsWith(x)";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace StartsWith() with EndsWith())");

        var invocationExpression =
            mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            invocationExpression.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("EndsWith");
    }

    [Fact]
    public void ShouldMutateTrimStart()
    {
        const string Expression = "testString.TrimStart()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace TrimStart() with TrimEnd())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("TrimEnd");
    }

    [Fact]
    public void ShouldMutateTrimEnd()
    {
        const string Expression = "testString.TrimEnd()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace TrimEnd() with TrimStart())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("TrimStart");
    }

    [Theory]
    [InlineData("Trim")]
    [InlineData("Substring")]
    [InlineData("ElementAt")]
    [InlineData("ElementAtOrDefault")]
    public void ShouldMutateReplaceWithEmptyString(string methodName)
    {
        var expression = $"testString.{methodName}()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe($"String Method Mutation (Replace {methodName}() with Empty String)");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        syntax.Token.ValueText.ShouldBe(string.Empty);
    }


    [Fact]
    public void ShouldMutateToUpper()
    {
        const string Expression = "testString.ToUpper()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace ToUpper() with ToLower())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("ToLower");
    }

    [Fact]
    public void ShouldMutateToLower()
    {
        const string Expression = "testString.ToLower()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace ToLower() with ToUpper())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("ToUpper");
    }

    [Fact]
    public void ShouldMutateToUpperInvariant()
    {
        const string Expression = "testString.ToUpperInvariant()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe(
            "String Method Mutation (Replace ToUpperInvariant() with ToLowerInvariant())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("ToLowerInvariant");
    }

    [Fact]
    public void ShouldMutateToLowerInvariant()
    {
        const string Expression = "testString.ToLowerInvariant()";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe(
            "String Method Mutation (Replace ToLowerInvariant() with ToUpperInvariant())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("ToUpperInvariant");
    }

    [Fact]
    public void ShouldMutatePadLeft()
    {
        const string Expression = "testString.PadLeft(10)";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace PadLeft() with PadRight())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("PadRight");
    }

    [Fact]
    public void ShouldMutatePadRight()
    {
        const string Expression = "testString.PadRight(10)";
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(Expression);
        var target = new StringMethodMutator();
        var result = target.ApplyMutations(expressionSyntax, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.String);
        mutation.DisplayName.ShouldBe("String Method Mutation (Replace PadRight() with PadLeft())");

        var syntax = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
        var memberAccessExpression =
            syntax.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();
        memberAccessExpression.Name.Identifier.ValueText.ShouldBe("PadLeft");
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
