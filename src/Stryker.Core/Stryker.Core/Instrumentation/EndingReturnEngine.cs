using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Injects 'return default(...)' statement at the end of a method
/// </summary>
internal class EndingReturnEngine: BaseEngine<BlockSyntax>
{
    public BlockSyntax InjectReturn(BlockSyntax block, TypeSyntax type)
    {
        // if we had no body or the last statement is a return, no need to add one, or this is an iterator method
        if (block == null
            || block.Statements.Count == 0
            || block.Statements.Last().IsKind(SyntaxKind.ReturnStatement)
            || block.Statements.Last().IsKind(SyntaxKind.ThrowStatement)
            || type.IsVoid()
            || block.ScanChildStatements(x => x.IsKind(SyntaxKind.YieldReturnStatement) || x.IsKind(SyntaxKind.YieldBreakStatement))
            || !block.ScanChildStatements(x => x.IsKind(SyntaxKind.ReturnStatement)))
        {
            return block;
        }

        block = block.AddStatements(SyntaxFactory.ReturnStatement( type == null ?
            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression).WithLeadingTrivia(SyntaxFactory.Space)
            : type.BuildDefaultExpression())).WithAdditionalAnnotations(Marker);

        return block;
    }

    protected override SyntaxNode Revert(BlockSyntax node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (!node.Statements.Last().IsKind(SyntaxKind.ReturnStatement))
        {
            throw new InvalidOperationException($"No return at the end of: {node}");
        }

        return node.WithStatements(node.Statements.Remove(node.Statements.Last())).WithoutAnnotations(Marker);
    }

}
