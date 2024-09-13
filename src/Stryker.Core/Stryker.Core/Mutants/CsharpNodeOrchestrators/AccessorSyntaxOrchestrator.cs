using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutation for Accessors (get/set). Its purpose is to convert arrow expression accessor to body statement form when needed.
/// </summary>
internal class AccessorSyntaxOrchestrator : BaseFunctionOrchestrator<AccessorDeclarationSyntax>
{
    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(AccessorDeclarationSyntax node) => (node.Body, node.ExpressionBody?.Expression);

    protected override ParameterListSyntax ParameterList(AccessorDeclarationSyntax node) => SyntaxFactory.ParameterList();

    protected override TypeSyntax ReturnType(AccessorDeclarationSyntax node) => node.ReturnType();

    protected override AccessorDeclarationSyntax SwitchToThisBodies(AccessorDeclarationSyntax node, BlockSyntax blockBody, ExpressionSyntax expressionBody)
        => expressionBody == null ? node.WithBody(blockBody).WithExpressionBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
        : node.WithBody(null).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expressionBody)).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
}
