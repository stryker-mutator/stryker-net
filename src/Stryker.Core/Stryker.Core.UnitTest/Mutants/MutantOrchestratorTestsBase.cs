using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Core.InjectedHelpers;
using Stryker.Abstractions.Options;
using System.Linq.Expressions;

namespace Stryker.Core.UnitTest.Mutants;

/// <summary>
/// This base class provides helper to test source file mutation
/// </summary>
public class MutantOrchestratorTestsBase : TestBase
{
    protected CsharpMutantOrchestrator Target;
    protected CodeInjection Injector = new();

    public MutantOrchestratorTestsBase() => Target = new CsharpMutantOrchestrator(new MutantPlacer(Injector), options: new StrykerOptions
    {
        MutationLevel = MutationLevel.Complete,
        OptimizationMode = OptimizationModes.CoverageBasedTest,
    });

    protected void ShouldMutateSourceToExpected(string actual, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(actual);
        
        var compilation = CSharpCompilation.Create(null)
                        .AddSyntaxTrees(syntaxTree);
        var model = compilation.GetSemanticModel(syntaxTree);
        var actualNode = Target.Mutate(syntaxTree, model);
        actual = actualNode.GetRoot().ToFullString();
        actual = actual.Replace(Injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual);
        actualNode.ShouldNotContainErrors();
        var expectedNode = CSharpSyntaxTree.ParseText(expected);
        actualNode.ShouldBeSemantically(expectedNode);
    }

    protected void ShouldMutateSourceInClassToExpected(string actual, string expected)
    {
        actual = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources;
class TestClass
{" + actual + @"}
";

        expected = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources;
class TestClass
{" + expected + @"}
";
        ShouldMutateSourceToExpected(actual, expected);
    }
}
