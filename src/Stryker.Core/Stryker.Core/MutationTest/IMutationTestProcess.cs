namespace Stryker.Core.MutationTest
{
    using System.Collections.Generic;
    using Mutants;


    public class MutantDiagnostic
    {
        private readonly IList<(MutantStatus status, IEnumerable<string> killingTests)> _runResults = new List<(MutantStatus status, IEnumerable<string> killingTests)>();

        public IList<(MutantStatus status, IEnumerable<string> killingTests)> RunResults => _runResults;

        public IEnumerable<string> CoveringTests { get; }
        public IEnumerable<int> MutationGroup { get; }
        public IReadOnlyMutant ConflictingMutant { get; set; }

        public IReadOnlyMutant DiagnosedMutant { get; }

        public MutantDiagnostic(Mutant mutant, IEnumerable<string> coveringTests, IEnumerable<int> mutationGroup)
        {
            CoveringTests = coveringTests;
            MutationGroup = mutationGroup;
            DiagnosedMutant = mutant;
        }

        public void DeclareResult(MutantStatus status, IEnumerable<string> killingTests) => _runResults.Add((status, killingTests));
    }

    public interface IMutationTestProcess
    {
        MutationTestInput Input { get; }
        void Mutate();
        StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest);
        MutantDiagnostic DiagnoseMutant(IEnumerable<Mutant> mutants, int mutantToDiagnose);
        void Restore();
        void GetCoverage();
        void FilterMutants();
    }
}
