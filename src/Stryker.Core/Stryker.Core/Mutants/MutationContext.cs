using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants.NodeOrchestrators;

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
            Disable = parent.Disable;
            Store = parent.Store;
        }

        public IEnumerable<Mutant> GenerateMutantsForNode(SyntaxNode node) => _mainOrchestrator.GenerateMutationsForNode(node, this);

        public INodeMutator FindHandler(SyntaxNode node) => _mainOrchestrator.GetHandler(node);

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }
        public bool Disable { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;
        public string[] FilteredMutators { get; set; }

        public MutationContext EnterStatic() => new(this) {InStaticValue = true};

        public MutationContext DisableMutation(bool mode = true) => new(this){Disable = mode};

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
