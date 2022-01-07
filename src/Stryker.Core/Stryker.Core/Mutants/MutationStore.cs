using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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
        private static readonly ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationStore>();
        protected List<Mutant> _expressionMutants = new();
        protected Stack<List<Mutant>> _statementMutants = new();
        protected Stack<List<Mutant>> _blockMutants = new();

        public bool HasBlockLevel => _blockMutants.Count > 0 && _blockMutants.Peek().Count > 0;
        public bool HasStatementLevel => (_statementMutants.Count > 0 && _statementMutants.Peek().Count > 0) || HasBlockLevel;

        public void StoreMutations(IEnumerable<Mutant> proposedMutations, MutationControl control)
        {
            var mutations = proposedMutations.Where(m => m.ResultStatus == MutantStatus.NotRun);
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
                        _logger.LogDebug($"{mutations.Count()} were generated but could not be injected as they cannot be controlled dynamically.");
                        foreach (var mutant in mutations)
                        {
                            _logger.LogDebug($"{mutant.Id}: {mutant.DisplayName}");
                            mutant.ResultStatus = MutantStatus.Ignored;
                            mutant.ResultStatusReason = "Unable to inject back mutations in source code.";
                        }
                        return;
                    }
                    _blockMutants.Peek().AddRange(mutations);
                    break;
            }
        }

        public StatementSyntax PlaceBlockMutations(SyntaxNode sourceNode, StatementSyntax block)
        {
            var result =
                MutantPlacer.PlaceStatementControlledMutations(sourceNode, block, _blockMutants.Peek());
            _blockMutants.Peek().Clear();
            return result;
        }

        public StatementSyntax PlaceStatementMutations(SyntaxNode sourceNode, StatementSyntax block)
        {
            var result =
                MutantPlacer.PlaceStatementControlledMutations(sourceNode, block, _statementMutants.Peek());
            _statementMutants.Peek().Clear();
            return result;
        }

        public ExpressionSyntax PlaceExpressionMutations(SyntaxNode sourceNode, ExpressionSyntax expression)
        {
            var result = MutantPlacer.PlaceExpressionControlledMutations(sourceNode, expression, _expressionMutants);
            _expressionMutants.Clear();
            return result;
        }

        public void EnterStatement()
        {
            _statementMutants.Push(new List<Mutant>());
        }

        public void LeaveStatement()
        {
            StoreMutations(_statementMutants.Pop(), MutationControl.Block);
            StoreMutations(_expressionMutants, MutationControl.Block);
            _expressionMutants.Clear();
        }

        public void EnterBlock()
        {
            _blockMutants.Push(new List<Mutant>());
        }

        public void LeaveBlock()
        {
            _blockMutants.Pop();
            _expressionMutants.Clear();
        }
    }
}
