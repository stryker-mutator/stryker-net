using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class AccessorSyntaxOrchestrator: NodeSpecificOrchestrator<AccessorDeclarationSyntax, SyntaxNode>
    {
        public AccessorSyntaxOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override SyntaxNode InjectMutations(AccessorDeclarationSyntax sourceNode, SyntaxNode targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context) as AccessorDeclarationSyntax;
            if (result?.Body == null && result?.ExpressionBody == null)
            {
                return result;
            }

            if (!context.HasStatementLevelMutant)
            {
                return result;
            }

            // we need to inject static marker
            if (result.Body == null)
            {
                result = MutantPlacer.ConvertExpressionToBody(result);
            }

            var converter = sourceNode.NeedsReturn()
                ? (Func<Mutation, StatementSyntax>) ((toConvert) =>
                    SyntaxFactory.ReturnStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert)))
                : (toConvert) =>
                    SyntaxFactory.ExpressionStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert));

            var newBody =
                MutantPlacer.PlaceStatementControlledMutations(result.Body,
                    context.StatementLevelControlledMutations.Union(context.BlockLevelControlledMutations).
                        Select( m => (m.Id, converter(m.Mutation))));
            context.BlockLevelControlledMutations.Clear();
            context.StatementLevelControlledMutations.Clear();
            return result.WithBody(SyntaxFactory.Block(newBody));
        }
    }
}
