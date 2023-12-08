using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stryker.Core.Options;

namespace Stryker.Core.Mutants
{
    public abstract class BaseMutantOrchestrator
    {
        public readonly StrykerOptions _options;

        public bool MustInjectCoverageLogic =>
            _options != null && _options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) &&
            !_options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest);

        public ICollection<Mutant> Mutants { get; set; }

        public int MutantCount { get; set; }

        protected BaseMutantOrchestrator(StrykerOptions options) => _options = options;

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
    public abstract class BaseMutantOrchestrator<T, TY> : BaseMutantOrchestrator
    {
        protected BaseMutantOrchestrator(StrykerOptions input) : base(input)
        {}

        public abstract T Mutate(T input, TY semanticModel);
    }
}
