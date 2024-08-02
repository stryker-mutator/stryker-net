using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stryker.Configuration;

namespace Stryker.Configuration.Mutants
{
    public abstract class BaseMutantOrchestrator
    {
        public readonly StrykerOptions Options;

        public bool MustInjectCoverageLogic =>
            Options != null && Options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) &&
            !Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest);

        public ICollection<Mutant> Mutants { get; set; }

        protected int MutantCount;

        protected BaseMutantOrchestrator(StrykerOptions options) => Options = options;

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        public virtual IReadOnlyCollection<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = Mutants;
            Mutants = new Collection<Mutant>();
            return (IReadOnlyCollection<Mutant>)tempMutants;
        }
    }

    /// <summary>
    /// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
    /// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
    /// </summary>
    /// <typeparam name="T">The type of syntax node to mutate</typeparam>
    /// <typeparam name="TY">Associated semantic model if any</typeparam>
    public abstract class BaseMutantOrchestrator<T, TY> : BaseMutantOrchestrator
    {
        protected BaseMutantOrchestrator(StrykerOptions input) : base(input)
        {}

        public abstract T Mutate(T input, TY semanticModel);
    }
}
