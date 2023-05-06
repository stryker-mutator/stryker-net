using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

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

        if (!context.HasStatementLevelMutant)
        {
            if (result.Body != null && sourceNode.NeedsReturn())
            {
                result = MutantPlacer.AddEndingReturn(result, sourceNode.ReturnType());
            }
            return result;
        }

        if (result.Body == null)
        {
            result = MutantPlacer.ConvertExpressionToBody(result);
        }

        var newBody = context.InjectBlockLevelExpressionMutation(result.Body, sourceNode.ExpressionBody!.Expression, sourceNode.NeedsReturn());
        result = result.WithBody(SyntaxFactory.Block(newBody));
        if (sourceNode.NeedsReturn())
        {
            result = MutantPlacer.AddEndingReturn(result, sourceNode.ReturnType());
        }
        return result;
    }
}
