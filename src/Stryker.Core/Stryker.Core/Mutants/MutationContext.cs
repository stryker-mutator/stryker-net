using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants
{

    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    internal class MutationContext
    {
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        public MutationStore Store = new();

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        private MutationContext(MutationContext parent)
        {
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
            Store = parent.Store;
            FilteredMutators = parent.FilteredMutators;
            FilterComment = parent.FilterComment;
        }

        public IEnumerable<Mutant> GenerateMutantsForNode(SyntaxNode node) => _mainOrchestrator.GenerateMutationsForNode(node, this);

        public INodeMutator FindHandler(SyntaxNode node) => _mainOrchestrator.GetHandler(node);

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public Mutator[] FilteredMutators { get; private set; }

        public string FilterComment { get; set; }

        public MutationContext EnterStatic() => new(this) { InStaticValue = true };

        public MutationContext Enter(MutationControl control)
        {
            switch (control)
            {
                case MutationControl.Statement:
                    Store.EnterStatement();
                    return this;
                case MutationControl.Block:
                    Store.EnterBlock();
                    return new MutationContext(this);
            }

            return this;
        }

        public MutationContext FilterMutators(bool mode, Mutator[] filteredMutators, bool newContext,
            string comment)
        {
            var result = newContext ? new MutationContext(this) : this;
            if (mode)
            {
                result.FilteredMutators = filteredMutators;
            }
            else if (result.FilteredMutators is not null)
            {
                result.FilteredMutators = result.FilteredMutators.Where(t => !filteredMutators.Contains(t)).ToArray();
            }

            if (mode)
            {
                result.FilterComment = comment;
            }
            return result;
        }

        public void Leave(MutationControl control)
        {
            switch (control)
            {
                case MutationControl.Statement:
                    Store.LeaveStatement();
                    break;
                case MutationControl.Block:
                    Store.LeaveBlock();
                    break;
            }
        }
    }
}
