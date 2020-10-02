using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class BaseMethodDeclarationOrchestrator<T> : NodeSpecificOrchestrator<T, BaseMethodDeclarationSyntax> where T: BaseMethodDeclarationSyntax
    {
        protected override bool NewContext => true;

        protected override BaseMethodDeclarationSyntax InjectMutations(T originalNode, BaseMethodDeclarationSyntax mutatedNode,
            MutationContext context)
        {
            mutatedNode = base.InjectMutations(originalNode, mutatedNode, context);
            if (mutatedNode.Body != null)
            {
                // add a return in case we changed the control flow
                return MutantPlacer.AddEndingReturn(mutatedNode) as T;
            }

            if (!context.HasStatementLevelMutant)
            {
                return mutatedNode;
            }

            // we need to move to a body version of the method
            mutatedNode = MutantPlacer.ConvertExpressionToBody(mutatedNode);

            StatementSyntax mutatedBlock = mutatedNode.Body;

            var converter = mutatedNode.NeedsReturn()
                ? (Func<Mutation, StatementSyntax>) ((toConvert) =>
                    SyntaxFactory.ReturnStatement(originalNode.ExpressionBody!.Expression.InjectMutation(toConvert)))
                : (toConvert) =>
                    SyntaxFactory.ExpressionStatement(originalNode.ExpressionBody!.Expression.InjectMutation(toConvert));

            mutatedBlock =
                MutantPlacer.PlaceStatementControlledMutations(mutatedBlock,
                    context.StatementLevelControlledMutations.Union(context.BlockLevelControlledMutations).
                        Select( m => (m.Id, converter(m.Mutation))));
            return mutatedNode.ReplaceNode(mutatedNode.Body!, 
                SyntaxFactory.Block(mutatedBlock));
        }

        public BaseMethodDeclarationOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}