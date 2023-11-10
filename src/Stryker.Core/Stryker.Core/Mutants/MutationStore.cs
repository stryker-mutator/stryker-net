using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Mutants
{
    public enum MutationControl
    {
        Expression,
        Statement,
        Block
    }

    /// <summary>
    /// Keeps track of mutations while parsing.
    /// It stores mutations according on which kind of syntaxnode mutation switching should occur (intra expression, statement or block).
    /// They also exposes mutation injection API but actual implementation is delegated to a mutant placer instance
    /// </summary>
    public class MutationStore
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationStore>();
        private readonly List<Mutant> _expressionMutants = new();
        private readonly Stack<List<Mutant>> _statementMutants = new();
        private readonly Stack<List<Mutant>> _blockMutants = new();
        private readonly MutantPlacer _placer;

        /// <summary>
        /// The length of the member access chain that is currently being parsed.
        /// </summary>
        public int MemberAccessLength { get; set; }

        /// <summary>
        /// true if there are block level mutations pending for injection
        /// </summary>
        public bool HasBlockLevel => _blockMutants.Count > 0 && _blockMutants.Peek().Count > 0;

        /// <summary>
        /// true if there are statement or block level mutations pending of injection
        /// </summary>
        public bool HasStatementLevel => (_statementMutants.Count > 0 && _statementMutants.Peek().Count > 0) || HasBlockLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="placer">MutationPlacer instance</param>
        public MutationStore(MutantPlacer placer) => _placer = placer;

        /// <summary>
        /// Stores the given mutations according to the expected control level
        /// </summary>
        /// <param name="proposedMutations">mutation list</param>
        /// <param name="control">expected control level</param>
        /// <remarks>does not store mutation  if the current context does not permit injection, e.g. it is not possible to store mutation if not inside executable code.
        /// rejected mutations are the logged. If this happen, this is probably an error from a mutator and/or the orchestration part of the logic.</remarks>
        public void StoreMutations(IEnumerable<Mutant> proposedMutations, MutationControl control)
        {
            var mutations = proposedMutations.Where(m => m.ResultStatus == MutantStatus.Pending);
            if (!mutations.Any())
            {
                return;
            }
            switch (control)
            {
                case MutationControl.Expression:
                    _expressionMutants.AddRange(mutations);
                    break;
                case MutationControl.Statement:
                    if (_statementMutants.Count == 0)
                    {
                        // try again at block level
                        StoreMutations(mutations, MutationControl.Block);
                        return;
                    }
                    _statementMutants.Peek().AddRange(mutations);
                    break;
                case MutationControl.Block:
                    if (_blockMutants.Count == 0)
                    {
                        // we have mutations that are going to be lost
                        Logger.LogDebug($"{mutations.Count()} were generated but could not be injected as they cannot be controlled dynamically.");
                        foreach (var mutant in mutations)
                        {
                            Logger.LogDebug($"{mutant.Id}: {mutant.DisplayName}");
                            mutant.ResultStatus = MutantStatus.Ignored;
                            mutant.ResultStatusReason = "Unable to inject back mutations in source code.";
                        }
                        return;
                    }
                    _blockMutants.Peek().AddRange(mutations);
                    break;
            }
        }

        /// <summary>
        /// Injects all pending block mutations, using the provided function for injection, and using 'if' mutation switching logic
        /// </summary>
        /// <param name="block">block where mutation will be inserted.</param>
        /// <param name="mutationFunc">function doing the actual injection logic</param>
        /// <returns>a block with the injected mutations</returns>
        /// <remarks>all pending block mutations are dropped after this call (clear the cache)</remarks>
        public StatementSyntax PlaceBlockMutations(StatementSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            var result = _placer.PlaceStatementControlledMutations(block, _blockMutants.Peek().Select(m => (m, mutationFunc(m.Mutation))));
            _blockMutants.Peek().Clear();
            return result;
        }

        /// <summary>
        /// Injects all pending statement mutations, using the provided function for injection, and using 'if' mutation switching logic at block level
        /// </summary>
        /// <param name="block">block where mutation will be inserted.</param>
        /// <param name="mutationFunc">function doing the actual injection logic</param>
        /// <returns>a block with the injected mutations</returns>
        /// <remarks>all pending statement mutations are dropped after this call (clear the cache)</remarks>
        public StatementSyntax PlaceStatementMutations(BlockSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            if (_statementMutants.Count == 0)
            {
                return block;
            }
            var result = _placer.PlaceStatementControlledMutations(block, _statementMutants.Peek().Select(m => (m, mutationFunc(m.Mutation))));
            _statementMutants.Peek().Clear();
            return result;
        }

        /// <summary>
        /// Injects all pending statement mutations, using the provided function for injection, and using 'if' mutation switching logic at statement level
        /// </summary>
        /// <param name="statement">statement where mutation will be inserted.</param>
        /// <param name="mutationFunc">function doing the actual injection logic</param>
        /// <returns>an if statement with the injected mutations</returns>
        /// <remarks>all pending statement mutations are dropped after this call (clear the cache)</remarks>
        public StatementSyntax PlaceStatementMutations(StatementSyntax statement, Func<Mutation, StatementSyntax> mutationFunc)
        {
            if (_statementMutants.Count == 0)
            {
                return statement;
            }
            var result = _placer.PlaceStatementControlledMutations(statement, _statementMutants.Peek().Select(m => (m, mutationFunc(m.Mutation))));
            _statementMutants.Peek().Clear();
            return result;
        }

        /// <summary>
        /// Injects all pending expression mutations, using the provided function for injection, and using ternary operator mutation switching logic at the expression level
        /// </summary>
        /// <param name="expression">expression where mutation will be inserted.</param>
        /// <param name="mutationFunc">function doing the actual injection logic</param>
        /// <returns>an expression with the injected mutations</returns>
        /// <remarks>all pending expression mutations are dropped after this call (clear the cache)</remarks>
        public ExpressionSyntax PlaceExpressionMutations(ExpressionSyntax expression, Func<Mutation, ExpressionSyntax> mutationFunc)
        {
            if (_expressionMutants.Count == 0)
            {
                return expression;
            }
            var result = _placer.PlaceExpressionControlledMutations(expression, _expressionMutants.Select(m => (m, mutationFunc(m.Mutation))));
            _expressionMutants.Clear();
            return result;
        }

        /// <summary>
        /// Must be called when entering a statement. This ensures mutations do not leak from a statement to some inner block/Statement
        /// </summary>
        public void EnterStatement() => _statementMutants.Push(new List<Mutant>());

        /// <summary>
        /// must be called when leaving a statement.
        /// Any non yet injected mutation will be promoted to block level
        /// </summary>
        public void LeaveStatement() => StoreMutations(_statementMutants.Pop(), MutationControl.Block);

        /// <summary>
        /// Must be called when entering a block. This ensures mutations do not leak from a block to some inner one
        /// </summary>
        public void EnterBlock() => _blockMutants.Push(new List<Mutant>());

        /// <summary>
        /// must be called when leaving a statement.
        /// Any non yet injected mutation will be promoted to block level
        /// </summary>
        /// <remarks>any pending block mutations are simply dropped. There is no known case where promotion is needed</remarks>
        public void LeaveBlock() => _blockMutants.Pop();
    }
}
