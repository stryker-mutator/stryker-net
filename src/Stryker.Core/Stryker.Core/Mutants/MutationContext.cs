using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (source code) context during mutation
    /// </summary>
    internal class MutationContext
    {
        private readonly MutantOrchestrator _mainOrchestrator;
        private readonly List<Mutant> _mutationsControlledByIfs = new List<Mutant>();

        public MutationContext(MutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public SyntaxNode Mutate(SyntaxNode subNode) => _mainOrchestrator.Mutate(subNode, this);

        public SyntaxNode MutateChildren(SyntaxNode node)
        {
            return _mainOrchestrator.MutateExpression(node, this);
        }
    
        public StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode, StatementSyntax nodeToReplace, ExpressionSyntax subExpression)
        {
            return _mainOrchestrator.MutateSubExpressionWithIfStatements(originalNode, nodeToReplace, subExpression, this);
        }

        public void StoreMutants(SyntaxNode node)
        {
            _mutationsControlledByIfs.AddRange(_mainOrchestrator.CaptureMutations(node, this));
        }

        public MutationContext EnterStatic()
        {
            return new MutationContext(_mainOrchestrator) { InStaticValue =  true};
        }

        public SyntaxNode MutateWithConditionals(ExpressionSyntax node, ExpressionSyntax mutateChildren)
        {
            return _mainOrchestrator.MutateSubExpressionWithConditional(node, mutateChildren, this);
        }

        public SyntaxNode InjectIfMutants(BlockSyntax original, BlockSyntax node)
        {
            if (_mutationsControlledByIfs.Count == 0)
            {
                return node;
            }
            var newBlock =
                SyntaxFactory.Block(_mainOrchestrator.PlaceMutantsAtBlockLevel(original, node, _mutationsControlledByIfs));
            _mutationsControlledByIfs.Clear();
            return newBlock;
        }

        public MutationContext Clone()
        {
            var inStatic = InStaticValue;
            return new MutationContext(_mainOrchestrator){InStaticValue = inStatic};
        }
    }
}