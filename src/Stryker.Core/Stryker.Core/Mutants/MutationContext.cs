using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;


namespace Stryker.Core.Mutants
{

    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    internal class MutationContext
    {
        private static readonly ILogger Logger;
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        public MutationStore Store = new();

        static MutationContext()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationContext>();
        }

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator) => _mainOrchestrator = mutantOrchestrator;

        private MutationContext(MutationContext parent)
        {
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
            Store = parent.Store;
            FilteredMutators = parent.FilteredMutators;
            FilterComment = parent.FilterComment;
        }

        public IEnumerable<Mutant> GenerateMutantsForNode(SyntaxNode node) =>
            _mainOrchestrator.GenerateMutationsForNode(node, this);

        public INodeMutator FindHandler(SyntaxNode node) => _mainOrchestrator.GetHandler(node);

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        /// <summary>
        /// True if orchestrators have to inject static usage tracing
        /// </summary>
        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public Mutator[] FilteredMutators { get; private set; }

        public string FilterComment { get; set; }

        /// <summary>
        /// true if there are pending statement or block level mutations
        /// </summary>
        public bool HasStatementLevelMutant => Store.HasStatementLevel;

        /// <summary>
        /// Call this to signal mutation occurs in static method or fields
        /// </summary>
        /// <returns>A new context</returns>
        public MutationContext EnterStatic() => new MutationContext(this) { InStaticValue = true };

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

        /// <summary>
        /// Register new statement level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddStatementLevel(IEnumerable<Mutant> mutants) =>
            Store.StoreMutations(mutants, MutationControl.Statement);

        /// <summary>
        /// Injects pending block level mutations for expression body method or functions
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="originalNode">Source node, used to generate mutations</param>
        /// <param name="needReturn">Set to true if the method has a return value. Expressions are transformed to return statement.</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectBlockLevelExpressionMutation(StatementSyntax mutatedNode,
            ExpressionSyntax originalNode,
            bool needReturn)
        {
            var wrapper = needReturn
                ? (Func<ExpressionSyntax, StatementSyntax>)SyntaxFactory.ReturnStatement
                : SyntaxFactory.ExpressionStatement;
            return Store.PlaceBlockMutations(mutatedNode, m =>
                wrapper(originalNode.InjectMutation(m)));

        }

        public MutationContext FilterMutators(bool mode, Mutator[] filteredMutators, bool newContext, string comment)
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
