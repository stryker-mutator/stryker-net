using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

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

        public bool HasBlockLevelMutant => _blockLevelControlledMutations.Count > 0;

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
            // inject any mutant that must be placed at the statement level
            if (subNode is BlockSyntax)
            {
                return InjectBlockLevelMutations(mutations, statement, context);
            }
            mutations = _mainOrchestrator.PlaceMutationsWithinIfControls(statement, mutations, context._statementLevelControlledMutations);
            _blockLevelControlledMutations.AddRange(context._blockLevelControlledMutations);
            return mutations;
        }

        public SyntaxNode InjectBlockLevelMutations(StatementSyntax mutatedBlock, StatementSyntax originalBlock, MutationContext context)
        {
            mutatedBlock =
                _mainOrchestrator.PlaceMutationsWithinIfControls( mutatedBlock,
                    context._statementLevelControlledMutations.Select( m => (m.Id, originalBlock.InjectMutation(m.Mutation))));
            // if this was a block, inject all block level controlled mutations
            if (context._blockLevelControlledMutations.Count == 0)
            {
                return mutatedBlock;
            }

            var newBlock =
                SyntaxFactory.Block(_mainOrchestrator.PlaceMutationsWithinIfControls(mutatedBlock,
                    context._blockLevelControlledMutations.Select( m => (m.Id, originalBlock.InjectMutation(m.Mutation)))));
            return newBlock;
        }

        public SyntaxNode InjectBlockLevelMutations(StatementSyntax mutatedBlock, ExpressionSyntax originalBlock, MutationContext context)
        {
            mutatedBlock =
                _mainOrchestrator.PlaceMutationsWithinIfControls( mutatedBlock,
                    context._statementLevelControlledMutations.Union(context._blockLevelControlledMutations).
                        Select( m => (m.Id, (StatementSyntax) SyntaxFactory.ReturnStatement(originalBlock.InjectMutation(m.Mutation)))));
            return SyntaxFactory.Block(mutatedBlock);
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
                        mutatedNode = _mainOrchestrator.PlaceMutationsWithinConditionalControls(expression, (ExpressionSyntax) mutatedNode, 
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
            return new MutationContext(_mainOrchestrator) { InStaticValue =  true};
        }

        private MutationContext Clone()
        {
            return new MutationContext(_mainOrchestrator){InStaticValue = InStaticValue};
        }
    }
}