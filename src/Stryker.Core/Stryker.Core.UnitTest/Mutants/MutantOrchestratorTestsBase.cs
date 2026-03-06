using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;

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
        Type[] typeToLoad = [typeof(object), typeof(List<>), typeof(Enumerable), typeof(Nullable<>)];
        var references = typeToLoad.Select( t=> MetadataReference.CreateFromFile(t.Assembly.Location)).Cast<MetadataReference>().ToArray();
        var compilation = CSharpCompilation.Create(null).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable))
                        .AddSyntaxTrees(syntaxTree).WithReferences(references);
        var actualNode = Target.Mutate(syntaxTree, compilation.GetSemanticModel(syntaxTree));
        actual = actualNode.GetRoot().ToFullString();
        actual = actual.Replace(Injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual);
        actualNode.ShouldNotContainErrors();
        var expectedNode = CSharpSyntaxTree.ParseText(expected);
        actualNode.ShouldBeSemantically(expectedNode);
    }

    protected void ShouldMutateSourceInClassToExpected(string actual, string expected)
    {
        var initBlock=@"using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources;";


        actual = string.Format(@"{0}
class TestClass
{{
{1}
}}
", initBlock, actual);

        expected = string.Format(@"{0}
class TestClass
{{
{1}
}}
", initBlock, expected);
        ShouldMutateSourceToExpected(actual, expected);
    }
}
