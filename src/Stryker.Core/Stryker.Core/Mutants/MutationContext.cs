using System.Collections.Generic;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    internal class MutationContext
    {
        private readonly MutantOrchestrator _mainOrchestrator;
        public readonly List<Mutant> BlockLevelControlledMutations = new List<Mutant>();
        public readonly List<Mutant> StatementLevelControlledMutations = new List<Mutant>();

        public MutationContext(MutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public bool HasBlockLevelMutant => BlockLevelControlledMutations.Count > 0;

        public MutationContext EnterStatic()
        {
            return new MutationContext(_mainOrchestrator) {InStaticValue = true};
        }

        public MutationContext Clone()
        {
            return new MutationContext(_mainOrchestrator) {InStaticValue = InStaticValue};
        }
    }
}