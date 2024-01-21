using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Stryker.Core.Helpers;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class PropertyDeclarationOrchestrator : NodeSpecificOrchestrator<PropertyDeclarationSyntax, BasePropertyDeclarationSyntax>, IInstrumentCode
{
    public PropertyDeclarationOrchestrator()
    {
        Marker = new SyntaxAnnotation(MutantPlacer.Injector, InstrumentEngineId);
        MutantPlacer.RegisterEngine(this, true);
    }

    /// <summary>
    /// Annotation to be added to the instrumented node
    /// </summary>
    private SyntaxAnnotation Marker { get; }

    public string InstrumentEngineId => GetType().ToString();

    protected override MutationContext PrepareContext(PropertyDeclarationSyntax node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Member));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());

    protected override BasePropertyDeclarationSyntax OrchestrateChildrenMutation(PropertyDeclarationSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        if (!node.IsStatic())
        {
            return base.OrchestrateChildrenMutation(node, semanticModel, context);
        }

        var children = node.ReplaceNodes(node.ChildNodes(), (original, _) =>
            MutateSingleNode(original, semanticModel, original == node.Initializer ? context.EnterStatic() : context));
        if (children.Initializer != null)
        {
            children = children.ReplaceNode(children.Initializer.Value,
                context.PlaceStaticContextMarker(children.Initializer.Value));
        }
        return children;
    }

    protected override BasePropertyDeclarationSyntax InjectMutations(PropertyDeclarationSyntax sourceNode,
        BasePropertyDeclarationSyntax targetNode, SemanticModel semanticModel, MutationContext context)
    {
        var result = base.InjectMutations(sourceNode, targetNode, semanticModel, context);
        var mutated = result as PropertyDeclarationSyntax;
        // if there is no statement level mutations or this is not an expression property declaration, we can stop
        if (!context.HasLeftOverMutations || mutated?.ExpressionBody == null)
        {
            return result;
        }

        // we need to convert the expression property to a regular property
        mutated = ConvertToBlockBody(mutated);
        var getter = mutated.GetAccessor();

        // and inject pending mutations in the getter's body.
        result = mutated.ReplaceNode(getter.Body!, context.InjectMutations(getter.Body, sourceNode.ExpressionBody!.Expression, true));
        return result;
    }

    public PropertyDeclarationSyntax ConvertToBlockBody(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration.ExpressionBody == null)
        {
            return propertyDeclaration;
        }

        var block = SyntaxFactory.Block(
            SyntaxFactory.ReturnStatement(
                propertyDeclaration.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space)));

        return propertyDeclaration.WithExpressionBody(null).WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(new []{
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block)}))).
            WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken))
            .WithAdditionalAnnotations(Marker);
    }

    public SyntaxNode RemoveInstrumentation(SyntaxNode node)
    {
        if (node is not PropertyDeclarationSyntax typedNode)
        {
            throw new InvalidOperationException($"Expected a PropertyDeclarationSyntax, found:\n{node.ToFullString()}.");
        }

        if (typedNode.AccessorList == null)
        {
            throw new InvalidOperationException($"Expected a property with a get propertyDeclaration {node}.");
        }

        var getter = typedNode.GetAccessor();
        if (getter != null)
        {
            if (getter.Body?.Statements.FirstOrDefault() is not ReturnStatementSyntax returnStatement)
            {
                throw new InvalidOperationException($"Expected a return statement here {getter.Body}.");
            }

            return typedNode.WithAccessorList(null)
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(returnStatement.Expression!))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        throw new InvalidOperationException($"Expected a get accessor {node}.");
    }
}
