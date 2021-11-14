namespace Stryker.Core.MutationTest
{
    using System.Collections.Generic;
    using Mutants;


    public class MutantDiagnostic
    {
        private IList<(MutantStatus status, IEnumerable<string> killingTests)> runResults = new List<(MutantStatus status, IEnumerable<string> killingTests)>();
        private IEnumerable<string> coveringTests ;

        public IList<(MutantStatus status, IEnumerable<string> killingTests)> RunResults => runResults;

        public IEnumerable<string> CoveringTests => coveringTests;

        public MutantDiagnostic(IEnumerable<string> coveringTests) => this.coveringTests = coveringTests;

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
