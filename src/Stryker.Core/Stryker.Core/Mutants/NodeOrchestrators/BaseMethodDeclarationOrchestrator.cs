using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handles Methods/properties' accessors/constructors and finalizers.
    /// </summary>
    /// <typeparam name="T">Type of the syntax node, must be derived from <see cref="BaseMethodDeclarationSyntax"/>.</typeparam>
    internal class BaseMethodDeclarationOrchestrator<T> : NodeSpecificOrchestrator<T, BaseMethodDeclarationSyntax> where T : BaseMethodDeclarationSyntax
    {
        protected override MutationContext PrepareContext(T node, MutationContext context)
        {
            context.Enter(MutationControl.Block);
            return base.PrepareContext(node, context);
        }

        protected override void RestoreContext(MutationContext context) => context.Leave(MutationControl.Block);

        /// <inheritdoc/>
        /// Inject mutations and convert expression body to block body if required.
        protected override BaseMethodDeclarationSyntax InjectMutations(T sourceNode, BaseMethodDeclarationSyntax targetNode,
            MutationContext context)
        {
            // find out parameters
            targetNode = base.InjectMutations(sourceNode, targetNode, context);
            if (targetNode.Body != null)
            {
                // inject initialization to default for all out parameters
                targetNode = sourceNode.ParameterList.Parameters.Where(p => p.Modifiers.Any(m => m.Kind() == SyntaxKind.OutKeyword))
                    .Aggregate(targetNode, (current, parameter) => MutantPlacer.AddDefaultInitialization(current, parameter.Identifier, parameter.Type));
                // add a return in case we changed the control flow
                return MutantPlacer.AddEndingReturn(targetNode) as T;
            }

            if (!context.Store.HasBlockLevel)
            {
                return targetNode;
            }

            // we need to move to a body version of the method
            targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);

            StatementSyntax mutatedBlock = targetNode.Body;

            var converter = targetNode.NeedsReturn()
                ? (Func<Mutation, StatementSyntax>)((toConvert) =>
                   SyntaxFactory.ReturnStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert)))
                : (toConvert) =>
                    SyntaxFactory.ExpressionStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert));

            mutatedBlock = context.Store.PlaceBlockMutations(mutatedBlock, converter);
            return targetNode.ReplaceNode(targetNode.Body!,
                SyntaxFactory.Block(mutatedBlock));
        }
    }
}
