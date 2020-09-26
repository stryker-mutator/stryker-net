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
        public readonly List<Mutant> BlockLevelControlledMutations = new List<Mutant>();
        public readonly List<Mutant> StatementLevelControlledMutations = new List<Mutant>();

        public MutationContext(MutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public bool HasBlockLevelMutant => BlockLevelControlledMutations.Count > 0;

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
            mutations = MutantPlacer.PlaceIfControlledMutations(mutations, context.StatementLevelControlledMutations.Select( m=>(m.Id, statement.InjectMutation(m.Mutation))));
            BlockLevelControlledMutations.AddRange(context.BlockLevelControlledMutations);
            return mutations;
        }

        public SyntaxNode InjectBlockLevelMutations(StatementSyntax mutatedBlock, StatementSyntax originalBlock, MutationContext context)
        {
            mutatedBlock =
                MutantPlacer.PlaceIfControlledMutations( mutatedBlock,
                    context.StatementLevelControlledMutations.Select( m => (m.Id, originalBlock.InjectMutation(m.Mutation))));
            // if this was a block, inject all block level controlled mutations
            if (context.BlockLevelControlledMutations.Count == 0)
            {
                return mutatedBlock;
            }

            var newBlock =
                SyntaxFactory.Block(MutantPlacer.PlaceIfControlledMutations(mutatedBlock,
                    context.BlockLevelControlledMutations.Select( m => (m.Id, originalBlock.InjectMutation(m.Mutation)))));
            return newBlock;
        }

        public SyntaxNode InjectBlockLevelMutations(StatementSyntax mutatedBlock, ExpressionSyntax originalBlock, MutationContext context)
        {
            mutatedBlock =
                MutantPlacer.PlaceIfControlledMutations( mutatedBlock,
                    context.StatementLevelControlledMutations.Union(context.BlockLevelControlledMutations).
                        Select( m => (m.Id, (StatementSyntax) SyntaxFactory.ReturnStatement(originalBlock.InjectMutation(m.Mutation)))));
            return SyntaxFactory.Block(mutatedBlock);
        }

        public SyntaxNode MutateNodeAndChildren(SyntaxNode node, bool statementLevelControlled = false)
        {
            var mutations = _mainOrchestrator.GenerateMutationsForNode(node, this);

            if (node is ExpressionSyntax expression)
            {
                if (!expression.ContainsDeclarations())
                {
                    if (statementLevelControlled)
                    {
                        StatementLevelControlledMutations.AddRange(mutations);
                        mutations = Enumerable.Empty<Mutant>();
                    }
                }
                else
                {
                    BlockLevelControlledMutations.AddRange(mutations);
                    mutations = Enumerable.Empty<Mutant>();
                }
            }
            else
            {
                StatementLevelControlledMutations.AddRange(mutations);
                mutations = Enumerable.Empty<Mutant>();
            }


            var mutatedNode = node.ReplaceNodes(node.ChildNodes(), (original, mutated) => Mutate(original));
            if (node is ExpressionSyntax mutatedExpression)
            {
                mutatedNode = MutantPlacer.PlaceExpressionControlledMutations(
                    (ExpressionSyntax) mutatedNode,
                    mutations.Select(m=> (m.Id, mutatedExpression.InjectMutation(m.Mutation))));
            }

            return mutatedNode;
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