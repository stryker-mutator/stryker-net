using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class ExpressionBodiedPropertyOrchestrator : BaseFunctionOrchestrator<PropertyDeclarationSyntax>
{
    protected override bool CanHandle(PropertyDeclarationSyntax t) => t.ExpressionBody!= null || (t.Initializer!=null && t.IsStatic());

    protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(PropertyDeclarationSyntax node) => (node.GetAccessor()?.Body, node.ExpressionBody?.Expression);

    protected override ParameterListSyntax ParameterList(PropertyDeclarationSyntax node) => SyntaxFactory.ParameterList();

    protected override TypeSyntax ReturnType(PropertyDeclarationSyntax node) => node.Type;

    protected override PropertyDeclarationSyntax SwitchToThisBodies(PropertyDeclarationSyntax node, BlockSyntax blockBody,
        ExpressionSyntax expressionBody)
    {
        if (expressionBody != null)
        {
            return node.WithAccessorList(null).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expressionBody)).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        return node.WithExpressionBody(null).WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(new []{
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, blockBody)}))).
            WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken));
    }

    protected override PropertyDeclarationSyntax OrchestrateChildrenMutation(PropertyDeclarationSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        if (node.Initializer == null)
        {
            return base.OrchestrateChildrenMutation(node, semanticModel, context);
        }

        var children = node.ReplaceNodes(node.ChildNodes(), (original, _) =>
        {
            var determinedContext = original == node.Initializer ? context.EnterStatic() : context;
            return determinedContext.FindHandler(original).Mutate(original, semanticModel, determinedContext);
        });
        return children.WithInitializer(children.Initializer.WithValue(context.PlaceStaticContextMarker(children.Initializer.Value)));
    }

}
