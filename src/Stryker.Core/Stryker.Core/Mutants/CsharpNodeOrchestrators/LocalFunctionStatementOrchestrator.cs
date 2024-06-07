using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class LocalFunctionStatementOrchestrator : BaseFunctionOrchestrator<LocalFunctionStatementSyntax>
{
    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(LocalFunctionStatementSyntax node) => (node.Body, node.ExpressionBody?.Expression);

    protected override ParameterListSyntax ParameterList(LocalFunctionStatementSyntax node) => node.ParameterList;

    protected override TypeSyntax ReturnType(LocalFunctionStatementSyntax node)
    {
        var returnType = node.ReturnType;
        if (node.Modifiers.ContainsAsyncKeyword())
        {
            var genericReturn = node.ReturnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            returnType = genericReturn?.TypeArgumentList.Arguments.First();
        }
        return returnType ?? RoslynHelper.VoidTypeSyntax();
    }

    protected override LocalFunctionStatementSyntax SwitchToThisBodies(LocalFunctionStatementSyntax node, BlockSyntax blockBody,
        ExpressionSyntax expressionBody) => node.WithBody(blockBody).WithExpressionBody(expressionBody is null ? null : SyntaxFactory.ArrowExpressionClause(expressionBody));        

}
