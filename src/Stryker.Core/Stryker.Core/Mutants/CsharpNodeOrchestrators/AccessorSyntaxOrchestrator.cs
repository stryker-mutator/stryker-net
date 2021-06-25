using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    /// <summary>
    /// Orchestrate mutation for Accessors. Its purpose is to convert arrow expression accessor to body statement form when needed.
    /// </summary>
    internal class AccessorSyntaxOrchestrator : NodeSpecificOrchestrator<AccessorDeclarationSyntax, SyntaxNode>
    {
        protected override SyntaxNode InjectMutations(AccessorDeclarationSyntax sourceNode, SyntaxNode targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context) as AccessorDeclarationSyntax;
            if (result?.Body == null && result?.ExpressionBody == null)
            {
                return result;
            }

            if (!context.Store.HasBlockLevel)
            {
                return result;
            }

            // we need to inject static marker
            if (result.Body == null)
            {
                result = MutantPlacer.ConvertExpressionToBody(result);
            }

            var converter = sourceNode.NeedsReturn()
                ? (Func<Mutation, StatementSyntax>)(toConvert =>
                   SyntaxFactory.ReturnStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert)))
                : (toConvert) =>
                    SyntaxFactory.ExpressionStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert));

            var newBody = context.Store.PlaceBlockMutations(result.Body, converter);
            return result.WithBody(SyntaxFactory.Block(newBody));
        }
    }
}
