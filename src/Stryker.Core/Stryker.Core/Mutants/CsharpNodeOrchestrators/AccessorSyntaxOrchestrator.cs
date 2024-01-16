using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutation for Accessors (get/set). Its purpose is to convert arrow expression accessor to body statement form when needed.
/// </summary>
internal class AccessorSyntaxOrchestrator : NodeSpecificOrchestrator<AccessorDeclarationSyntax, SyntaxNode>
{
    protected override SyntaxNode InjectMutations(AccessorDeclarationSyntax sourceNode, SyntaxNode targetNode, SemanticModel semanticModel, MutationContext context)
    {
        var result = base.InjectMutations(sourceNode, targetNode, semanticModel, context) as AccessorDeclarationSyntax;
        if (result?.Body == null && result?.ExpressionBody == null)
        {
            // no implementation provided
            return result;
        }
        // no mutations to inject
        if (!context.HasLeftOverMutations)
        {
            if (result.Body != null && sourceNode.NeedsReturn())
            {
                result = MutantPlacer.AddEndingReturn(result, sourceNode.ReturnType());
            }
            return result;
        }

        if (result.Body == null)
        {
            // there are statement level mutations, we  need to convert the expression to a block
            result = MutantPlacer.ConvertExpressionToBody(result);
        }

        var newBody = context.InjectMutations(result.Body, sourceNode.ExpressionBody!.Expression, sourceNode.NeedsReturn());
        result = result.WithBody(newBody.AsBlock());
        if (sourceNode.NeedsReturn())
        {
            result = MutantPlacer.AddEndingReturn(result, sourceNode.ReturnType());
        }
        return result;
    }

    protected override MutationContext PrepareContext(AccessorDeclarationSyntax node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Member));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
