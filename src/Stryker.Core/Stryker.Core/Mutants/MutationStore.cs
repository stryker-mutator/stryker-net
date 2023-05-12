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
    /// Keeps track of mutations while parsing
    /// </summary>
    public class MutationStore
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationStore>();
        private readonly List<Mutant> _expressionMutants = new();
        private readonly Stack<List<Mutant>> _statementMutants = new();
        private readonly Stack<List<Mutant>> _blockMutants = new();
        private readonly MutantPlacer _placer;

        public MutationStore(MutantPlacer placer)
        {
            _placer = placer;
        }

        public bool HasBlockLevel => _blockMutants.Count > 0 && _blockMutants.Peek().Count > 0;
        public bool HasStatementLevel => (_statementMutants.Count > 0 && _statementMutants.Peek().Count > 0) || HasBlockLevel;

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

        public StatementSyntax PlaceBlockMutations(StatementSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            var result = _placer.PlaceStatementControlledMutations(block, _blockMutants.Peek().Select(m => (m, mutationFunc(m.Mutation))));
            _blockMutants.Peek().Clear();
            return result;
        }

        public StatementSyntax PlaceStatementMutations(StatementSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            if (_statementMutants.Count == 0)
            {
                return block;
            }
            var result = _placer.PlaceStatementControlledMutations(block, _statementMutants.Peek().Select(m => (m, mutationFunc(m.Mutation))));
            _statementMutants.Peek().Clear();
            return result;
        }

        public ExpressionSyntax PlaceExpressionMutations(ExpressionSyntax expression, Func<Mutation, ExpressionSyntax> converter)
        {
            var result = _placer.PlaceExpressionControlledMutations(expression, _expressionMutants.Select(m => (m, converter(m.Mutation))));
            _expressionMutants.Clear();
            return result;
        }

        public void EnterStatement() => _statementMutants.Push(new List<Mutant>());

        public void LeaveStatement()
        {
            StoreMutations(_statementMutants.Pop(), MutationControl.Block);
            StoreMutations(_expressionMutants, MutationControl.Block);
            _expressionMutants.Clear();
        }

        public void EnterBlock() => _blockMutants.Push(new List<Mutant>());

        public void LeaveBlock()
        {
            _blockMutants.Pop();
            _expressionMutants.Clear();
        }
    }
}
