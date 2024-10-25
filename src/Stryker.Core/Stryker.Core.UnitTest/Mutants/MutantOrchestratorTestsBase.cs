using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Core.InjectedHelpers;
using Stryker.Abstractions.Options;

namespace Stryker.Core.UnitTest.Mutants;

/// <summary>
/// This base class provides helper to test source file mutation
/// </summary>
public class MutantOrchestratorTestsBase : TestBase
{
    protected CSharpMutantOrchestrator Target;
    protected CodeInjection Injector = new();

    public MutantOrchestratorTestsBase() => Target = new CSharpMutantOrchestrator(new MutantPlacer(Injector), options: new StrykerOptions
    {
        MutationLevel = MutationLevel.Complete,
        OptimizationMode = OptimizationModes.CoverageBasedTest,
    });

    protected void ShouldMutateSourceToExpected(string actual, string expected)
    {
        var actualNode = Target.Mutate(CSharpSyntaxTree.ParseText(actual), null);
        actual = actualNode.GetRoot().ToFullString();
        actual = actual.Replace(Injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual);
        var expectedNode = CSharpSyntaxTree.ParseText(expected);
        actualNode.ShouldBeSemantically(expectedNode);
        actualNode.ShouldNotContainErrors();
    }

    protected void ShouldMutateSourceInClassToExpected(string actual, string expected)
    {
        actual = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    class TestClass
    {" + actual + @"}
}";

        expected = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    class TestClass
    {" + expected + @"}
}";
        ShouldMutateSourceToExpected(actual, expected);
    }
}
