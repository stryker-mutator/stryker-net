using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stryker.Core.Options;

namespace Stryker.Core.Mutants
{
    public abstract class MutantOrchestrator<T> : MutantOrchestrator
    {
        protected  MutantOrchestrator() : base(null)
        {
        }
        protected MutantOrchestrator(StrykerOptions input) : base(input)
        {
        }

        public abstract T Mutate(T input);
    }

    public abstract class MutantOrchestrator
    {
        public readonly StrykerOptions _options;

        public bool MustInjectCoverageLogic =>
            _options != null && _options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) &&
            !_options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest);

        public ICollection<Mutant> Mutants { get; set; }
        public int MutantCount { get; set; }

        protected MutantOrchestrator(StrykerOptions options)
        {
            _options = options;
        }

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
}
