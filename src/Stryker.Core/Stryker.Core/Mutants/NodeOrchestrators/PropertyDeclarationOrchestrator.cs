using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class PropertyDeclarationOrchestrator: NodeSpecificOrchestrator<PropertyDeclarationSyntax, BasePropertyDeclarationSyntax>
    {

        public PropertyDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override BasePropertyDeclarationSyntax OrchestrateChildrenMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            if (!node.IsStatic())
            {
                return base.OrchestrateChildrenMutation(node, context);
            }

            return node.ReplaceNodes(node.ChildNodes(), (original, _) =>
                MutantOrchestrator.Mutate(original, original == node.Initializer ? context.EnterStatic() : context));
        }

        protected override BasePropertyDeclarationSyntax InjectMutations(PropertyDeclarationSyntax sourceNode,
            BasePropertyDeclarationSyntax targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context);
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
                    SyntaxFactory.Block( context.InjectBlockLevelExpressionMutation( getter.Body, sourceNode.ExpressionBody!.Expression, true)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
            return result;
        }
    }
}
