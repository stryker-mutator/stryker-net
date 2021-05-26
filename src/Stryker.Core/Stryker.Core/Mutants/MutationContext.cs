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

    public class MutationStore
    {
        protected List<Mutant> _expressionMutants = new();
        protected Stack<List<Mutant>> _statementMutants = new();
        protected Stack<List<Mutant>> _blockMutants = new();

        public bool HasBlockLevel => _blockMutants.Count > 0 && _blockMutants.Peek().Count > 0;
        public bool HasStatementLevel => _statementMutants.Count > 0 && _statementMutants.Peek().Count > 0;
        public void StoreMutations(IEnumerable<Mutant> mutations, MutationControl control)
        {
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
                        // TOTO log lost mutations
                        return;
                    }
                    _blockMutants.Peek().AddRange(mutations);
                    break;
            }
        }

        public StatementSyntax PlaceBlockMutations(StatementSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            var result=
                MutantPlacer.PlaceStatementControlledMutations(block,
                    _blockMutants.Peek()
                        .Select(m => (m.Id, mutationFunc(m.Mutation))));
            _blockMutants.Peek().Clear();
            return result;
        }

        public StatementSyntax PlaceStatementMutations(StatementSyntax block, Func<Mutation, StatementSyntax> mutationFunc)
        {
            var result=
                MutantPlacer.PlaceStatementControlledMutations(block,
                    _statementMutants.Peek()
                        .Select(m => (m.Id, mutationFunc(m.Mutation))));
            _statementMutants.Peek().Clear();
            return result;
        }

        public ExpressionSyntax PlaceExpressionMutations(ExpressionSyntax expression,
            Func<Mutation, ExpressionSyntax> converter)
        {
            var result= MutantPlacer.PlaceExpressionControlledMutations(expression,
                _expressionMutants.Select(m => (m.Id, converter(m.Mutation))));
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

    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    public class MutationContext
    {
        private static readonly ILogger _logger;
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        public MutationStore Store = new();

        static MutationContext()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationContext>();
        }

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        private MutationContext(MutationContext parent)
        {
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;
        
        public bool HasStatementLevelMutant => Store.HasStatementLevel;

        public MutationContext EnterStatic() => new(this) {InStaticValue = true};

        public MutationContext Clone() => new(this){Store = this.Store};
    }
}
