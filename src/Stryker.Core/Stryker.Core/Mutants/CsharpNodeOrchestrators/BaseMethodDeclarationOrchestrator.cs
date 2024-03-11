using System.Linq;
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
    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(T node) => (node.Body, node.ExpressionBody?.Expression);

    protected override ParameterListSyntax ParameterList(T node) => node.ParameterList;

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
