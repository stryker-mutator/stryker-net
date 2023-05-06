using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Handles Methods/properties' accessors/constructors and finalizers.
/// </summary>
/// <typeparam name="T">Type of the syntax node, must be derived from <see cref="BaseMethodDeclarationSyntax"/>.</typeparam>
internal class BaseMethodDeclarationOrchestrator<T> : NodeSpecificOrchestrator<T, BaseMethodDeclarationSyntax> where T : BaseMethodDeclarationSyntax
{
    protected override MutationContext PrepareContext(T node, MutationContext context)
        => base.PrepareContext(node, context.Enter(MutationControl.Block));

    protected override void RestoreContext(MutationContext context)
    {
        context.Leave(MutationControl.Block);
        base.RestoreContext(context);
    }

    /// <inheritdoc/>
    /// Inject mutations and convert expression body to block body if required.
    protected override BaseMethodDeclarationSyntax InjectMutations(T sourceNode, BaseMethodDeclarationSyntax targetNode,
        MutationContext context)
    {
        targetNode = base.InjectMutations(sourceNode, targetNode, context);

        if (targetNode.Body == null)
        {
            if (targetNode.ExpressionBody == null)
            {
                // only a definition (eg interface)
                return targetNode;
            }

            // this is an expression body method
            if (!context.HasStatementLevelMutant)
            {
                // there is no statement or block level mutant, so the method control flow is not changed by mutations
                // there is no need to change the method in any may
                return targetNode;
            }

            // we need to convert it to expression body form
            targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);

            // we need to inject pending block (and statement) level mutations
            targetNode = targetNode.WithBody(
                SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(targetNode.Body, sourceNode.ExpressionBody?.Expression, sourceNode.NeedsReturn())));
        }
        else
        {
            // we add an ending return, just in case
            targetNode = MutantPlacer.AddEndingReturn(targetNode);
        }

        // inject initialization to default for all out parameters
        targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Body, sourceNode.ParameterList.Parameters.Where(p =>
            p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))));
        return targetNode;
    }
}
