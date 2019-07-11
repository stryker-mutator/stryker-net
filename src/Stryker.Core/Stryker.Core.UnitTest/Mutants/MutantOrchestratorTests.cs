using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Stryker.Core.InjectedHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantOrchestratorTests
    {
        private string CurrentDirectory { get; set; }
        private MutantOrchestrator Target { get; set; }

        public MutantOrchestratorTests()
        {
            Target = new MutantOrchestrator(new Collection<IMutator>
            {
                new AddMutator(),
                new AssignmentStatementMutator()
            });
            CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        [Theory]
        [InlineData("Mutator_IfStatementsShouldBe_Nested_IN.cs", "Mutator_IfStatementsShouldBe_Nested_OUT.cs")]
        [InlineData("Mutator_SyntaxShouldBe_IfStatement_IN.cs", "Mutator_SyntaxShouldBe_IfStatement_OUT.cs")]
        [InlineData("Mutator_SyntaxShouldBe_ConditionalStatement_IN.cs", "Mutator_SyntaxShouldBe_ConditionalStatement_OUT.cs")]
        [InlineData("Mutator_AssignStatements_IN.cs", "Mutator_AssignStatements_OUT.cs")]
        public void Mutator_TestResourcesInputShouldBecomeOutputBasicMutators(string inputFile, string outputFile)
        {
            string source = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + inputFile);
            string expected = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + outputFile).Replace("StrykerNamespace", CodeInjection.HelperNamespace);

            var actualNode = Target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }

        [Theory]
        [InlineData("Mutator_Basic_IN.cs", "Mutator_Basic_OUT.cs")]
        public void Mutator_TestResourcesInputShouldBecomeOutputAllMutators(string inputFile, string outputFile)
        {
            string source = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + inputFile);
            string expected = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + outputFile).Replace("StrykerNamespace", CodeInjection.HelperNamespace);

            var target = new MutantOrchestrator();
            var actualNode = target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }

        [Theory]
        [InlineData("Mutator_FromActualProject_IN.cs", "Mutator_FromActualProject_OUT.cs", 18, 5, 14, 12, 31)]
        [InlineData("Mutator_KnownComplexCases_IN.cs", "Mutator_KnownComplexCases_OUT.cs", 19, 2, 14, 6, 24)]
        [InlineData("Mutator_Chained_Mutation_IN.cs", "Mutator_Chained_Mutation_OUT.cs", 7, 2, 13, 3, 13)]
        public void Mutator_TestResourcesInputShouldBecomeOutputForFullScope(string inputFile, string outputFile,
            int nbMutants, int mutant1Id, int mutant1Location, int mutant2Id, int mutant2Location)
        {
            var source = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + inputFile);
            var expected = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + outputFile).Replace("StrykerNamespace", CodeInjection.HelperNamespace);
            var target = new MutantOrchestrator();

            var actualNode = target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();

            var mutants = target.GetLatestMutantBatch().ToList();
            mutants.Count.ShouldBe(nbMutants);
            mutants[mutant1Id].Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(mutant1Location, "Location was lost in mutation");
            mutants[mutant2Id].Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(mutant2Location, "Location was lost in mutation");
        }

        [Theory]
        [InlineData("Mutator_TwoMutationsInOneStatmentShouldBeMade.cs", 2)]
        [InlineData("Mutator_NoMutationsShouldBeMade.cs", 0)]
        public void Mutator_NumberOfMutationsShouldBeCorrect(string inputFile, int numberOfMutations)
        {
            var source = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + inputFile);
            var tree = CSharpSyntaxTree.ParseText(source);

            Target.Mutate(tree.GetRoot());

            Target.GetLatestMutantBatch().Count().ShouldBe(numberOfMutations);
        }

        [Theory]
        [InlineData("Mutator_Flag_MutatedStatics_IN.cs")]
        public void Mutator_ShouldFlagMutatedStatics(string inputFile)
        {
            var source = File.ReadAllText(CurrentDirectory + "/Mutants/TestResources/" + inputFile);
            var tree = CSharpSyntaxTree.ParseText(source);

            var target = new MutantOrchestrator();
            var result = target.Mutate(tree.GetRoot());
            
            var mutants = target.GetLatestMutantBatch().ToList();

            mutants.Count.ShouldBe(3);

            mutants[0].IsStaticValue.ShouldBe(true);
            mutants[1].IsStaticValue.ShouldBe(false);
            mutants[2].IsStaticValue.ShouldBe(true);
        }
    }
}
