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
internal class BaseMethodDeclarationOrchestrator<T> : BaseFunctionOrchestrator<T> where T : BaseMethodDeclarationSyntax
{
        /*
    /// <inheritdoc/>
    protected override BaseMethodDeclarationSyntax InjectMutations(T sourceNode, BaseMethodDeclarationSyntax targetNode,
        SemanticModel semanticModel, MutationContext context)
    {
        if (targetNode.Body == null)
        {
            if (targetNode.ExpressionBody == null)
            {
                // only a definition (e.g. interface)
                return targetNode;
            }

            // this is an expression body method
            if (!context.HasLeftOverMutations)
            {
                // there is no statement or block level mutant, so the method control flow is not changed by mutations
                // there is no need to change the method in any may
                return targetNode;
            }

            // we need to convert it to expression body form
            targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);

            // we need to inject pending block (and statement) level mutations
            targetNode = targetNode.WithBody(context.InjectMutations(targetNode.Body, sourceNode.ExpressionBody?.Expression, sourceNode.NeedsReturn()));
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
    */
    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(T node) => (node.Body, node.ExpressionBody?.Expression);

    protected override ParameterListSyntax Parameters(T node) => node.ParameterList;

    protected override TypeSyntax ReturnType(T node)
    {
        var returnType = node.ReturnType();
        if (node.Modifiers.ContainsAsyncKeyword())
        {
            var genericReturn = node.ReturnType().DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            returnType = genericReturn?.TypeArgumentList.Arguments.First();
        }
        return returnType ?? RoslynHelper.VoidTypeSyntax();
    }

    protected override T SwitchToThisBodies(T node, BlockSyntax blockBody, ExpressionSyntax expressionBody)
    {
        if (expressionBody == null)
        {
            return (T) node.WithBody(blockBody).WithExpressionBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
        }

        return (T) node.WithBody(null).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expressionBody)).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }
}
