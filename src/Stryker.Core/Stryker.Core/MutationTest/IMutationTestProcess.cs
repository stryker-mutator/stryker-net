namespace Stryker.Core.MutationTest
{
    using System.Collections.Generic;
    using Mutants;


    public class MutantDiagnostic
    {
        private IList<(MutantStatus status, IEnumerable<string> killingTests)> runResults = new List<(MutantStatus status, IEnumerable<string> killingTests)>();

        public IList<(MutantStatus status, IEnumerable<string> killingTests)> RunResults => runResults;

        public IEnumerable<string> CoveringTests { get; }
        public IEnumerable<int> MutationGroup { get; }
        public int ConflictingMutant { get; set; }

        public MutantDiagnostic(IEnumerable<string> coveringTests, IEnumerable<int> mutationGroup)
        {
            CoveringTests = coveringTests;
            MutationGroup = mutationGroup;
        }

        public void DeclareResult(MutantStatus status, IEnumerable<string> killingTests) => runResults.Add((status, killingTests));
    }

    public interface IMutationTestProcess
    {
        MutationTestInput Input { get; }
        void Mutate();
        StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest);
        MutantDiagnostic DiagnoseMutant(IList<Mutant> mutants, int mutantToDiagnose);
        void Restore();
        void GetCoverage();
        void FilterMutants();
    }
}
