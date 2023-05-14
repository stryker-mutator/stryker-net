using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
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
        private readonly MutationStore _store;

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
            _store = new MutationStore(_mainOrchestrator.Placer);
        }

        private MutationContext(MutationContext parent)
        {
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
            _store = parent._store;
            FilteredMutators = parent.FilteredMutators;
            FilterComment = parent.FilterComment;
        }

        /// <summary>
        /// Call this to generate mutations using active mutators.
        /// </summary>
        /// <param name="node"><see cref="SyntaxNode"/> to mutate.</param>
        /// <returns>A list of mutants.</returns>
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

        internal Mutator[] FilteredMutators { get; private set; }

        internal string FilterComment { get; set; }

        /// <summary>
        /// true if there are pending statement or block level mutations
        /// </summary>
        public bool HasStatementLevelMutant => _store.HasStatementLevel;

        /// <summary>
        /// Call this to signal mutation occurs in static method or fields
        /// </summary>
        /// <returns>A new context</returns>
        public MutationContext EnterStatic() => new(this) { InStaticValue = true };

        /// <summary>
        /// Call this when beginning of a syntax structure that can control mutations (expression, statement, block)
        /// </summary>
        /// <param name="control">type of structure (see <see cref="MutationControl"/>)</param>
        /// <returns>The context to use moving forward.</returns>
        /// <remarks>You must use a <see cref="Leave(MutationControl)"/>when leaving the context.</remarks>
        public MutationContext Enter(MutationControl control)
        {
            switch (control)
            {
                case MutationControl.Statement:
                    _store.EnterStatement();
                    return this;
                case MutationControl.Block:
                    _store.EnterBlock();
                    return new MutationContext(this);
            }

            return this;
        }

        /// <summary>
        /// Call this when leaving a control syntax structure
        /// </summary>
        /// <param name="control">type of structure (see <see cref="MutationControl"/>)</param>
        /// <remarks>A call must match a previous call to <see cref="Enter(MutationControl)"/></remarks>
        public void Leave(MutationControl control)
        {
            switch (control)
            {
                case MutationControl.Statement:
                    _store.LeaveStatement();
                    break;
                case MutationControl.Block:
                    _store.LeaveBlock();
                    break;
            }
        }

        /// <summary>
        /// Register new statement level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddExpressionLevel(IEnumerable<Mutant> mutants) =>
            _store.StoreMutations(mutants, MutationControl.Expression);

        /// <summary>
        /// Register new statement level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddStatementLevel(IEnumerable<Mutant> mutants) =>
            _store.StoreMutations(mutants, MutationControl.Statement);

        /// <summary>
        /// Register new block level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddBlockLevel(IEnumerable<Mutant> mutants) =>
            _store.StoreMutations(mutants, MutationControl.Block);

        /// <summary>
        /// Injects pending expression level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="sourceNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public ExpressionSyntax InjectExpressionLevel(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode)
            => _store.PlaceExpressionMutations(mutatedNode, m => sourceNode.InjectMutation(m));

        public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) => _mainOrchestrator.Placer.PlaceStaticContextMarker(block);

        public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax block) => _mainOrchestrator.Placer.PlaceStaticContextMarker(block);

        /// <summary>
        /// Injects pending statement level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="sourceNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectStatementLevel(StatementSyntax mutatedNode, StatementSyntax sourceNode)
            => _store.PlaceStatementMutations(mutatedNode, sourceNode.InjectMutation);

        /// <summary>
        /// Injects pending block level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="originalNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectBlockLevel(StatementSyntax mutatedNode, StatementSyntax originalNode) => _store.PlaceBlockMutations(mutatedNode, m => originalNode.InjectMutation(m));

        /// <summary>s
        /// Injects pending block level mutations for expression body method or functions
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="originalNode">Source node, used to generate mutations</param>
        /// <param name="needReturn">Set to true if the method has a return value. Expressions are transformed to return statement.</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectBlockLevelExpressionMutation(StatementSyntax mutatedNode, ExpressionSyntax originalNode, bool needReturn)
        {
            var wrapper = needReturn
                ? (Func<ExpressionSyntax, StatementSyntax>)SyntaxFactory.ReturnStatement
                : SyntaxFactory.ExpressionStatement;

            if (_store.HasStatementLevel)
            {
                mutatedNode = _store.PlaceStatementMutations(mutatedNode, m => wrapper(originalNode.InjectMutation(m)));
            }

            return _store.PlaceBlockMutations(mutatedNode, m => wrapper(originalNode.InjectMutation(m)));
        }

        /// <summary>
        /// Enable/Disable a list of mutators.
        /// </summary>
        /// <param name="mode">true to disable mutators, false to (re)enable some</param>
        /// <param name="filteredMutators">list of mutators</param>
        /// <param name="newContext">true to create a new context</param>
        /// <param name="comment">comment used for ignored mutators</param>
        /// <returns>a context with an updated list of active mutators</returns>
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
    }
}
