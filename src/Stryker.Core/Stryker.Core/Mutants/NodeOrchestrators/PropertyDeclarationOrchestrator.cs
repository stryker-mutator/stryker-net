using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class PropertyDeclarationOrchestrator: NodeSpecificOrchestrator<PropertyDeclarationSyntax, BasePropertyDeclarationSyntax>
    {
        protected override MutationContext PrepareContext(PropertyDeclarationSyntax node, MutationContext context)
        {
            context.Enter(MutationControl.Block);
            return base.PrepareContext(node, context);
        }

        protected override void RestoreContext(MutationContext context)
        {
            context.Leave(MutationControl.Block);
            base.RestoreContext(context);
        }

        protected override BasePropertyDeclarationSyntax OrchestrateChildrenMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            if (!node.IsStatic())
            {
                return base.OrchestrateChildrenMutation(node, context);
            }

            return node.ReplaceNodes(node.ChildNodes(), (original, _) =>
                MutateSingleNode(original, original == node.Initializer ? context.EnterStatic() : context));
        }

        protected override BasePropertyDeclarationSyntax InjectMutations(PropertyDeclarationSyntax sourceNode,
            BasePropertyDeclarationSyntax targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context);
            if (!context.Store.HasBlockLevel)
            {
                return result;
            }
            var mutated = result as PropertyDeclarationSyntax;

            if (mutated?.ExpressionBody == null)
            {
                return result;
            }
            mutated = MutantPlacer.ConvertPropertyExpressionToBodyAccessor(mutated);
            var getter = mutated.GetAccessor();

            var converter = (Func<Mutation, StatementSyntax>)(toConvert =>
                SyntaxFactory.ReturnStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert)));

            result = mutated.ReplaceNode(getter.Body!, SyntaxFactory.Block( context.Store.PlaceBlockMutations(getter.Body, converter)))
                .WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken));
            return result;
        }
    }
}
