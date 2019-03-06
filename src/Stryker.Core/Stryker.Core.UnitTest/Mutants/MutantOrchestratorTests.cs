using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantOrchestratorTests
    {
        private Collection<IMutator> _mutators { get; set; }
        private string _currentDirectory { get; set; }
        private MutantOrchestrator _target { get; set; } 

        public MutantOrchestratorTests()
        {
            _target = new MutantOrchestrator(new Collection<IMutator>
            {
                new AddMutator(),
                new AssignmentStatementMutator()
            });
            _currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        [Theory]
        [InlineData("Mutator_IfStatementsShouldBe_Nested_IN.cs", "Mutator_IfStatementsShouldBe_Nested_OUT.cs")]
        [InlineData("Mutator_SyntaxShouldBe_IfStatement_IN.cs", "Mutator_SyntaxShouldBe_IfStatement_OUT.cs")]
        [InlineData("Mutator_SyntaxShouldBe_ConditionalStatement_IN.cs", "Mutator_SyntaxShouldBe_ConditionalStatement_OUT.cs")]
        [InlineData("Mutator_AssignStatements_IN.cs", "Mutator_AssignStatements_OUT.cs")]
        public void Mutator_TestResourcesInputShouldBecomeOutput(string inputFile, string outputFile)
        {
            string source = File.ReadAllText(_currentDirectory + "/Mutants/TestResources/" + inputFile);
            string expected = File.ReadAllText(_currentDirectory + "/Mutants/TestResources/" + outputFile);

            var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
        }

        [Theory]
        [InlineData("Mutator_FromActualProject_IN.cs", "Mutator_FromActualProject_OUT.cs", 24, 5, 14, 12, 30)]
        [InlineData("Mutator_KnownComplexCases_IN.cs", "Mutator_KnownComplexCases_OUT.cs", 10, 2, 16, 6, 21)]
        public void Mutator_TestResourcesInputShouldBecomeOutputForFullScope(string inputFile, string outputFile, 
            int nbMutants, int mutant1Id, int mutant1Location, int mutant2Id, int mutant2Location)
        {
            string source = File.ReadAllText(_currentDirectory + "/Mutants/TestResources/" + inputFile);
            string expected = File.ReadAllText(_currentDirectory + "/Mutants/TestResources/" + outputFile);
            var target = new MutantOrchestrator();
            var actualNode = target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);

            var mutants = target.GetLatestMutantBatch().ToList();
            mutants.Count.ShouldBe(nbMutants);
            mutants[mutant1Id].Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(mutant1Location);
            mutants[mutant2Id].Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(mutant2Location);
        }

        [Theory]
        [InlineData("Mutator_TwoMutationsInOneStatmentShouldBeMade.cs", 2)]
        [InlineData("Mutator_NoMutationsShouldBeMade.cs", 0)]
        public void Mutator_NumberOfMutationsShouldBeCorrect(string inputFile, int numberOfMutations)
        {
            string source = File.ReadAllText(_currentDirectory + "/Mutants/TestResources/" + inputFile);
            var tree = CSharpSyntaxTree.ParseText(source);

            var result = _target.Mutate(tree.GetRoot());

            _target.GetLatestMutantBatch().Count().ShouldBe(numberOfMutations);
        }
    }
}
