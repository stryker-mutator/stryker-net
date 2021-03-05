using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    public class MutationContext: IDisposable
    {
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        private readonly MutationContext _ancestor;
        public readonly List<Mutant> ExpressionLevelMutations = new List<Mutant>();
        public readonly List<Mutant> BlockLevelControlledMutations = new List<Mutant>();
        public readonly List<Mutant> StatementLevelControlledMutations = new List<Mutant>();

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        private MutationContext(MutationContext parent)
        {
            _ancestor = parent;
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public bool HasBlockLevelMutant => BlockLevelControlledMutations.Count > 0;
        
        public bool HasStatementLevelMutant => StatementLevelControlledMutations.Count > 0 || HasBlockLevelMutant;

        public MutationContext EnterStatic()
        {
            return new MutationContext(this) {InStaticValue = true};
        }

        public MutationContext Clone()
        {
            return new MutationContext(this);
        }

        public void Dispose()
        {
            if (_ancestor == null)
            {
                return;
            }
            // copy the pending mutation to the enclosing context
            _ancestor.StatementLevelControlledMutations.AddRange(StatementLevelControlledMutations);
            _ancestor.BlockLevelControlledMutations.AddRange(BlockLevelControlledMutations);
        }
    }
}
