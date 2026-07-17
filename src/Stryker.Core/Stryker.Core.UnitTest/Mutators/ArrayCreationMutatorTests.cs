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
public class ArrayCreationMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new ArrayCreationMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    public void ShouldRemoveValuesFromArrayCreation()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new int[] { 1, 3 }") as ArrayCreationExpressionSyntax;

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, null);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldRemoveValuesFromImplicitArrayCreation()
    {
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new [] { 1, 3 }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
        replacement.Type.ElementType.ToString().ShouldBe("int");
    }

    [TestMethod]
    public void ShouldRemoveValuesFromImplicitArrayCreationWithDouble()
    {
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new [] { 1.0, 2.0 }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
        replacement.Type.ElementType.ToString().ShouldBe("double");
    }

    [TestMethod]
    public void ShouldRemoveValuesFromImplicitJaggedArrayCreation()
    {
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new [] { new int[] { 1 } }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
        replacement.Type.ToString().ShouldBe("int[][]");
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationWithAnonymousType()
    {
        // The element type is anonymous and cannot be written explicitly in C#, so no mutant is emitted.
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new[] { new { x = 1 } }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationWithNestedAnonymousType()
    {
        // The element type is an array of an anonymous type; the anonymous type cannot be written
        // explicitly, so no mutant is emitted. Exercises the recursive ContainsAnonymousType guard.
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new[] { new[] { new { x = 1 } } }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationWithTupleContainingAnonymousType()
    {
        // The element type is a tuple containing an anonymous type; the anonymous type cannot be
        // written explicitly, so no mutant is emitted. Exercises the INamedTypeSymbol.TypeArguments
        // recursion branch of ContainsAnonymousType.
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new[] { (1, new { z = 2 }) }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationWithAnonymousContainingType()
    {
        // The element type is a nested type whose containing generic type is closed over an anonymous
        // type (Outer<anonymous>.Inner). The anonymous type cannot be written explicitly, so no mutant
        // is emitted. Exercises the ContainingType recursion branch of ContainsAnonymousType.
        var source = """
            class Outer<T> { public class Inner { } }
            class Factory { public static Outer<T>.Inner Make<T>(T t) => default; }
            class C { void M() { var x = new[] { Factory.Make(new { z = 1 }) }; } }
            """;
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("t")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(tree);
        var semanticModel = compilation.GetSemanticModel(tree);
        var expressionSyntax = tree.GetRoot().DescendantNodes().OfType<ImplicitArrayCreationExpressionSyntax>().First();

        var result = new ArrayCreationMutator().ApplyMutations(expressionSyntax, semanticModel);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldRemoveValuesFromImplicitArrayCreationWithTuple()
    {
        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression("new[] { (1, \"a\") }");

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, semanticModel);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
        replacement.Type.ElementType.ToString().ShouldBe("(int, string)");
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationWithoutSemanticModel()
    {
        var expressionSyntax = SyntaxFactory.ParseExpression("new [] { 1, 3 }") as ImplicitArrayCreationExpressionSyntax;
        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(expressionSyntax, null);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateEmptyInitializer()
    {
        var arrayCreationExpression = SyntaxFactory.ParseExpression("new int[] { }") as ArrayCreationExpressionSyntax;
        var stackallocArrayCreationExpression = SyntaxFactory.ParseExpression("stackalloc int[] { }") as StackAllocArrayCreationExpressionSyntax;

        var target = new ArrayCreationMutator();

        var result1 = target.ApplyMutations(arrayCreationExpression, null);
        var result2 = target.ApplyMutations(stackallocArrayCreationExpression, null);

        result1.ShouldBeEmpty();
        result2.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateStackallocArrays()
    {
        var stackallocArrayCreationExpression = SyntaxFactory.ParseExpression("stackalloc int[] { 1 }") as StackAllocArrayCreationExpressionSyntax;

        var target = new ArrayCreationMutator();

        var result = target.ApplyMutations(stackallocArrayCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<StackAllocArrayCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
    }

    private static (SemanticModel semanticModel, ImplicitArrayCreationExpressionSyntax expression)
        CreateSemanticModelFromExpression(string input)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            $$"""
              class TestClass {
                  void ArrayMethod() {
                      var x = {{input}};
                  }
              }
              """);

        var compilation = CSharpCompilation.Create("TestAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);

        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<ImplicitArrayCreationExpressionSyntax>().First();
        return (semanticModel, expression);
    }
}
