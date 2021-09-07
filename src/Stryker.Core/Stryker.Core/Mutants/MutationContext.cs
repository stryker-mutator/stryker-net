using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    public class MutationContext: IDisposable
    {
        private static readonly ILogger Logger;
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        private readonly MutationContext _ancestor;
        private readonly List<Mutant> _expressionLevelMutations = new List<Mutant>();
        private readonly List<Mutant> _blockLevelControlledMutations = new List<Mutant>();
        private readonly List<Mutant> _statementLevelControlledMutations = new List<Mutant>();

        static MutationContext()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationContext>();
        }

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator) => _mainOrchestrator = mutantOrchestrator;

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

        /// <summary>
        /// True if orchestrators have to inject static usage tracing
        /// </summary>
        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        /// <summary>
        /// True if there are pending block level mutations
        /// </summary>
        public bool HasBlockLevelMutant => _blockLevelControlledMutations.Count > 0;

        /// <summary>
        /// true if there are pending statement or block level mutations
        /// </summary>
        public bool HasStatementLevelMutant => _statementLevelControlledMutations.Count > 0 || HasBlockLevelMutant;

        /// <summary>
        /// Call this to signal mutation occurs in static method or fields
        /// </summary>
        /// <returns>A new context</returns>
        public MutationContext EnterStatic() => new MutationContext(this) {InStaticValue = true};

        /// <summary>
        /// Clone the context. Prevent mutations to drift 
        /// </summary>
        /// <returns>A copy of the context</returns>
        public MutationContext Clone() => new MutationContext(this);

        /// <summary>
        /// Promote all pending statement level mutations to block level.
        /// </summary>
        public void PromoteToBlockLevel()
        {
            _blockLevelControlledMutations.AddRange(_statementLevelControlledMutations);
            _statementLevelControlledMutations.Clear();
        }

        /// <summary>
        /// Register new expression level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddExpressionLevel(IEnumerable<Mutant> mutants) => _expressionLevelMutations.AddRange(mutants);

        /// <summary>
        /// Register new statement level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddStatementLevel(IEnumerable<Mutant> mutants) =>
            _statementLevelControlledMutations.AddRange(mutants);

        /// <summary>
        /// Register new block level mutations
        /// </summary>
        /// <param name="mutants"></param>
        public void AddBlockLevel(IEnumerable<Mutant> mutants) =>
            _blockLevelControlledMutations.AddRange(mutants);

        /// <summary>
        /// Injects pending expression level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="sourceNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public ExpressionSyntax InjectExpressionLevel(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode)
        {
            var result = MutantPlacer.PlaceExpressionControlledMutations(
                mutatedNode,
                _expressionLevelMutations.Select(m => (m.Id, sourceNode.InjectMutation(m.Mutation))));
            _expressionLevelMutations.Clear();
            return result;
        }

        /// <summary>
        /// Injects pending statement level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="sourceNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectStatementLevel(StatementSyntax mutatedNode, StatementSyntax sourceNode)
        {
            var mutated = MutantPlacer.PlaceStatementControlledMutations(mutatedNode,
                _statementLevelControlledMutations.Select( m => (m.Id, sourceNode.InjectMutation(m.Mutation))));
            _statementLevelControlledMutations.Clear();
            return mutated;
        }

        /// <summary>
        /// Injects pending block level mutations.
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="originalNode">Source node, used to generate mutations</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectBlockLevel(StatementSyntax mutatedNode, StatementSyntax originalNode)
        {
            var result= MutantPlacer.PlaceStatementControlledMutations(mutatedNode,
                _statementLevelControlledMutations.Union(_blockLevelControlledMutations).Select(m =>
                    (m.Id, originalNode.InjectMutation(m.Mutation))));
            _statementLevelControlledMutations.Clear();
            _blockLevelControlledMutations.Clear();
            return result;
        }

        /// <summary>
        /// Injects pending block level mutations for expression body method or functions
        /// </summary>
        /// <param name="mutatedNode">Target node that will contain the mutations</param>
        /// <param name="originalNode">Source node, used to generate mutations</param>
        /// <param name="needReturn">Set to true if the method has a return value. Expressions are transformed to return statement.</param>
        /// <returns>A mutated node containing the mutations.</returns>
        public StatementSyntax InjectBlockLevelExpressionMutation(StatementSyntax mutatedNode, ExpressionSyntax originalNode,
            bool needReturn)
        {
            var wrapper = needReturn
                ? (Func<ExpressionSyntax, StatementSyntax>)SyntaxFactory.ReturnStatement
                : SyntaxFactory.ExpressionStatement;
            var result= MutantPlacer.PlaceStatementControlledMutations(mutatedNode,
                _statementLevelControlledMutations.Union(_blockLevelControlledMutations).Select(m =>
                    (m.Id, wrapper(originalNode.InjectMutation(m.Mutation)))));
            _statementLevelControlledMutations.Clear();
            _blockLevelControlledMutations.Clear();
            return result;
        }


        /// <summary>
        /// Discards all pending mutations. Discarded mutations are marked as such.
        /// </summary>
        public void Discard()
        {
            if (!HasStatementLevelMutant) return;
            // some mutants 
            Logger.LogDebug($"{_blockLevelControlledMutations.Count+_statementLevelControlledMutations.Count} mutations were not injected.");
            foreach (var mutant in _blockLevelControlledMutations.Union(_statementLevelControlledMutations))
            {
                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Stryker was not able to inject mutation in code.";
            }
            _blockLevelControlledMutations.Clear();
            _statementLevelControlledMutations.Clear();

        }

        /// <summary>
        /// Dispose the context. Promote pending mutations to the higher context when relevant, otherwise Discard them.
        /// </summary>
        public void Dispose()
        {
            if (_ancestor == null)
            {
                Discard();
                return;
            }
            // copy the pending mutation to the enclosing context
            _ancestor._statementLevelControlledMutations.AddRange(_statementLevelControlledMutations);
            _ancestor._blockLevelControlledMutations.AddRange(_blockLevelControlledMutations);
        }
    }
}
