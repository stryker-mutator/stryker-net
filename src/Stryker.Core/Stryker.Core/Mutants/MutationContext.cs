using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

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

        private SyntaxNode Mutate(SyntaxNode subNode)
        {
            if (!(subNode is StatementSyntax statement))
            {
                return _mainOrchestrator.Mutate(subNode, this);
            }
            // we are about to mutate a statement
            // create statement local context
            var context = Clone();
            var mutations = _mainOrchestrator.Mutate(subNode, context) as StatementSyntax;
            if (subNode is BlockSyntax blockSyntax)
            {
                // if this was a block, inject all block level controlled mutations
                if (context._blockLevelControlledMutations.Count == 0)
                {
                    return mutations;
                }
                var newBlock =
                    SyntaxFactory.Block(_mainOrchestrator.PlaceMutantWithinIfControls(blockSyntax, mutations, context._blockLevelControlledMutations));
                return newBlock;
            }
            // simple statement, inject all statement level controlled mutations and aggregates block level mutations.
            _blockLevelControlledMutations.AddRange(context._blockLevelControlledMutations);
            mutations = _mainOrchestrator.PlaceMutantWithinIfControls(statement, mutations,
                context._statementLevelControlledMutations);
            return mutations;

        }

        public SyntaxNode MutateNodeAndChildren(SyntaxNode node, bool statementLevelControlled = false)
        {
            SyntaxNode mutatedNode;
            if (node is ExpressionSyntax expression)
            {
                if (!expression.ContainsDeclarations())
                {
                    if (!statementLevelControlled)
                    {
                        // the mutations can be controlled by conditional operator
                        mutatedNode = node.TrackNodes(expression.ChildNodes());
                        mutatedNode = _mainOrchestrator.PlaceMutantWithinConditionalControls(expression, (ExpressionSyntax)mutatedNode,
                            _mainOrchestrator.GenerateMutantsForNode(node, this));
                    }
                    else
                    {
                        _statementLevelControlledMutations.AddRange(_mainOrchestrator.GenerateMutantsForNode(node, this));
                        mutatedNode = node.TrackNodes(node.ChildNodes());
                    }
                }
                else
                {
                    _blockLevelControlledMutations.AddRange(_mainOrchestrator.GenerateMutantsForNode(node, this));
                    mutatedNode = node.TrackNodes(node.ChildNodes());
                }
            }
            else
            {
                _statementLevelControlledMutations.AddRange(_mainOrchestrator.GenerateMutantsForNode(node, this));
                mutatedNode = node.TrackNodes(node.ChildNodes());
            }

            var mutatedNode1 = mutatedNode;
            foreach (var child in node.ChildNodes().ToList())
            {
                var mutatedChild = Mutate(child);
                if (SyntaxFactory.AreEquivalent(child, mutatedChild))
                {
                    continue;
                }

                mutatedNode1 = mutatedNode1.ReplaceNode(mutatedNode1.GetCurrentNode(child), mutatedChild);
            }

            return mutatedNode1;
        }

        public MutationContext EnterStatic()
        {
            return new MutationContext(_mainOrchestrator) { InStaticValue = true };
        }

        private MutationContext Clone()
        {
            return new MutationContext(_mainOrchestrator) { InStaticValue = InStaticValue };
        }
    }
}