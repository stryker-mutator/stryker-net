using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class PropertyDeclarationOrchestrator : NodeSpecificOrchestrator<PropertyDeclarationSyntax, BasePropertyDeclarationSyntax>
    {
        protected override MutationContext PrepareContext(PropertyDeclarationSyntax node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Member));

        protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave(MutationControl.Member));

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
            if (!context.HasStatementLevelMutant || mutated?.ExpressionBody == null)
            {
                return result;
            }

            // we need to convert the expression property to a regular property
            mutated = MutantPlacer.ConvertPropertyExpressionToBodyAccessor(mutated);
            var getter = mutated.GetAccessor();

            // and inject pending mutations in the getter's body.
            result = mutated.ReplaceNode(getter.Body!,
                    SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(getter.Body, sourceNode.ExpressionBody!.Expression, true)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
            return result;
        }
    }
}
