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
public class ObjectCreationMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new ObjectCreationMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [TestMethod]
    [DataRow("new List<int> { 1, 3 }")]
    [DataRow("new Collection<int> { 1, 3 }")]
    [DataRow(@"new Dictionary<int, StudentName>()
        {
            { 111, new StudentName { FirstName='Foo', LastName='Bar', ID=211 } }
        };")]
    public void ShouldRemoveValuesFromCollectionInitializer(string initializer)
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

        var target = new ObjectCreationMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.Initializer);

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldRemoveValuesFromObjectInitializer()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new SomeClass { SomeProperty = SomeValue }") as ObjectCreationExpressionSyntax;

        var target = new ObjectCreationMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Object initializer mutation");
        mutation.Type.ShouldBe(Mutator.Initializer);

        var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("new List<int> { }")]
    [DataRow("new SomeClass { }")]
    public void ShouldNotMutateEmptyInitializer(string initializer)
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

        var target = new ObjectCreationMutator();

        var result = target.ApplyMutations(objectCreationExpression, null);

        result.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("public required int Value { get; set; }")]
    [DataRow("public required int Value;")]
    [DataRow("public int Other { get; set; } public required int Value { get; set; }")]
    public void ShouldPreserveRequiredMembersWhenMutatingObjectInitializer(string members)
    {
        var (semanticModel, expression) = CreateSemanticModel(
            $$"""
              class Target { {{members}} }
              class Caller { void M() { var t = new Target { Value = 1 }; } }
              """);

        var result = new ObjectCreationMutator().ApplyMutations(expression, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.Type.ShouldBe(Mutator.Initializer);
        var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldHaveSingleItem().ToString().ShouldBe("Value=default!");
    }

    [TestMethod]
    public void ShouldPreserveRequiredMembersFromBaseTypeWhenMutatingObjectInitializer()
    {
        var (semanticModel, expression) = CreateSemanticModel(
            """
            class Base { public required int Value { get; set; } }
            class Derived : Base { public int Other { get; set; } }
            class Caller { void M() { var t = new Derived { Value = 1, Other = 2 }; } }
            """);

        var result = new ObjectCreationMutator().ApplyMutations(expression, semanticModel).ToList();

        var mutation = result.ShouldHaveSingleItem();
        var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
        replacement.Initializer.Expressions.ShouldHaveSingleItem().ToString().ShouldBe("Value=default!");
    }

    [TestMethod]
    public void ShouldMutateObjectInitializerWhenTypeHasNoRequiredMembers()
    {
        var (semanticModel, expression) = CreateSemanticModel(
            """
            class Target { public int Value { get; set; } }
            class Caller { void M() { var t = new Target { Value = 1 }; } }
            """);

        var result = new ObjectCreationMutator().ApplyMutations(expression, semanticModel).ToList();

        result.ShouldHaveSingleItem().Type.ShouldBe(Mutator.Initializer);
    }

    private static (SemanticModel semanticModel, ObjectCreationExpressionSyntax expression) CreateSemanticModel(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("TestAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Single();
        return (semanticModel, expression);
    }
}
