using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class AnonymousFunctionExpressionOrchestrator : BaseFunctionOrchestrator<AnonymousFunctionExpressionSyntax>
{
    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(AnonymousFunctionExpressionSyntax node) => (node.Block, node.ExpressionBody);

    protected override ParameterListSyntax ParameterList(AnonymousFunctionExpressionSyntax node) => node switch {ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.ParameterList,
        SimpleLambdaExpressionSyntax simpleLambda => SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(simpleLambda.Parameter)),
        _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
    };

    protected override TypeSyntax ReturnType(AnonymousFunctionExpressionSyntax node) => null;

    protected override AnonymousFunctionExpressionSyntax SwitchToThisBodies(AnonymousFunctionExpressionSyntax node, BlockSyntax blockBody,
        ExpressionSyntax expressionBody) =>
        node.WithBody(blockBody).WithExpressionBody(expressionBody);
}
