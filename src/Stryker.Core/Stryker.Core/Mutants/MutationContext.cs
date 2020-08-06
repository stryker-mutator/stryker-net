using System.Collections.Generic;
using System.Linq;
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
        private readonly List<Mutant> _blockLevelControlledMutations = new List<Mutant>();
        private readonly List<Mutant> _statementLevelControlledMutations = new List<Mutant>();

        public MutationContext(MutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }


        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public SyntaxNode Mutate(SyntaxNode subNode)
        {
            if (!(subNode is StatementSyntax statement))
            {
                return _mainOrchestrator.Mutate(subNode, this);
            }
            var context = Clone();
            var mutations = _mainOrchestrator.Mutate(subNode, context) as StatementSyntax;
            if (subNode is BlockSyntax blockSyntax)
            {
                return context.InjectIfMutants(blockSyntax,
                    (BlockSyntax) mutations);
            }

            _blockLevelControlledMutations.AddRange(context._blockLevelControlledMutations);
            mutations = _mainOrchestrator.PlaceMutantsAtBlockLevel(statement, mutations,
                context._statementLevelControlledMutations);
            return mutations;

        }

        public SyntaxNode MutateChildren(SyntaxNode node)
        {
            // Nothing to mutate, dig further
            var childCopy = node.TrackNodes(node.ChildNodes().ToList().Append(node));
            var mutated = false;

            foreach (var child in node.ChildNodes().ToList())
            {
                var mutatedChild = Mutate(child);
                if (child != mutatedChild)
                {
                    childCopy = childCopy.ReplaceNode(childCopy.GetCurrentNode(child), mutatedChild);
                    mutated = true;
                }
            }

            return mutated ? childCopy : node;
        }
    
        public void StoreMutants(SyntaxNode node)
        {
            if (node is ExpressionSyntax expression && expression.ContainsDeclarations())
            {
                _blockLevelControlledMutations.AddRange(_mainOrchestrator.GenerateMutationsForNode(node, this));
            }
            else if (node is AssignmentExpressionSyntax || node is PostfixUnaryExpressionSyntax)
            {
                _statementLevelControlledMutations.AddRange(_mainOrchestrator.GenerateMutationsForNode(node, this));
            }
        }

        public MutationContext EnterStatic()
        {
            return new MutationContext(_mainOrchestrator) { InStaticValue =  true};
        }

        public SyntaxNode MutateWithConditionals(ExpressionSyntax originalNode, ExpressionSyntax mutatedNode)
        {
            return _mainOrchestrator.MutateSubExpressionWithConditional(originalNode, mutatedNode, this);
        }

        public SyntaxNode InjectIfMutants(BlockSyntax original, BlockSyntax node)
        {
            if (_blockLevelControlledMutations.Count == 0)
            {
                return node;
            }
            var newBlock =
                SyntaxFactory.Block(_mainOrchestrator.PlaceMutantsAtBlockLevel(original, node, _blockLevelControlledMutations));
            _blockLevelControlledMutations.Clear();
            return newBlock;
        }

        public MutationContext Clone()
        {
            var inStatic = InStaticValue;
            return new MutationContext(_mainOrchestrator){InStaticValue = inStatic};
        }
    }
}