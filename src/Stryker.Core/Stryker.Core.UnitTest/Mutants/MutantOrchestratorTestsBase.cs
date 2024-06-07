using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;

namespace Stryker.Core.UnitTest.Mutants
{
    /// <summary>
    /// This base class provides helper to test source file mutation
    /// </summary>
    public class MutantOrchestratorTestsBase : TestBase
    {
        protected CsharpMutantOrchestrator _target;
        protected CodeInjection _injector = new();

        public MutantOrchestratorTestsBase()
        {
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Complete,
                OptimizationMode = OptimizationModes.CoverageBasedTest,
            };
            _target = new CsharpMutantOrchestrator(new MutantPlacer(_injector), options: options);
        }

        protected void ShouldMutateSourceToExpected(string actual, string expected)
        {
            var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(actual), null);
            actual = actualNode.GetRoot().ToFullString();
            actual = actual.Replace(_injector.HelperNamespace, "StrykerNamespace");
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
}
