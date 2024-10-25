using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class DefaultParameterMutatorTests
{
    [TestMethod]
    public void ShouldBeMutationLevelComplete()
    {
        var options = new StrykerOptions();
        var orchestratorMock = new Mock<ICSharpMutantOrchestrator>();
        var target = new DefaultParameterMutator(orchestratorMock.Object, options);
        target.MutationLevel.ShouldBe(MutationLevel.Complete);
    }

    [TestMethod]
    public void ShouldMutateDefaultParameter()
    {
        var source = @"
using System;

class Program
{
    void Method()
    {
        DefaultParameterMethod();
    }

    void DefaultParameterMethod(string parameter = ""default"")
    {
        Console.WriteLine(parameter);
    }
}"";
";
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        GetMutations(syntaxTree).ShouldHaveSingleItem();
    }

    [TestMethod]
    public void ShouldNotMutateExplicitlyUsedDefaultParameter()
    {
        var source = @"
using System;

class Program
{
    void Method()
    {
        DefaultParameterMethod(""hello world"");
    }

    void DefaultParameterMethod(string parameter = ""default"")
    {
        Console.WriteLine(parameter);
    }
}"";
";
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        GetMutations(syntaxTree).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateExplicitlyUsedNamedDefaultParameter()
    {
        var source = @"
using System;

class Program
{
    void Method()
    {
        DefaultParameterMethod(parameter2: ""hello world"");
    }

    void DefaultParameterMethod(string parameter1 = ""default1"", string parameter2 = ""default2"", string parameter3 = ""default3"")
    {
        Console.WriteLine(parameter);
    }
}"";
";
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var mutations = GetMutations(syntaxTree);

        mutations.Count().ShouldBe(2);
        mutations.ShouldNotContain(x => x.DisplayName.Contains("parameter2"));
    }

    [TestMethod]
    public void ShouldMutateImplicitlyUsedDefaultParameter()
    {
        var source = @"
using System;

class Program
{
    void Method()
    {
        DefaultParameterMethod(""Hello world"");
    }

    void DefaultParameterMethod(string parameter1 = ""default"", string parameter2 = ""default2"", string parameter3 = ""default3"")
    {
        Console.WriteLine(parameter);
    }
}"";
";
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var mutations = GetMutations(syntaxTree);

        mutations.Count().ShouldBe(2);
        mutations.ShouldNotContain(x => x.DisplayName.Contains("parameter1"));
    }

    private IEnumerable<Mutation> GetMutations(SyntaxTree tree)
    {
        var invocations = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>();
        var options = new StrykerOptions()
        {
            MutationLevel = MutationLevel.Complete
        };
        var orchestratorMock = new Mock<ICSharpMutantOrchestrator>();
        orchestratorMock.SetupGet(m => m.Mutators).Returns(new List<IMutator> { new StringMutator() });

        var target = new DefaultParameterMutator(orchestratorMock.Object, options);

        var semanticModel = GetSemanticModel(tree);

        return invocations.SelectMany(invocation => target.ApplyMutations(invocation, semanticModel));
    }

    private static SemanticModel GetSemanticModel(SyntaxTree tree)
    {
        var compilation = CSharpCompilation.Create("Test")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(tree);
        return compilation.GetSemanticModel(tree);
    }
}
