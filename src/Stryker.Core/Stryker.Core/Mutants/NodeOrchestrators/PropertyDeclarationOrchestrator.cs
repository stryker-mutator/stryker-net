using System.Linq;
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

        protected override BasePropertyDeclarationSyntax InjectMutations(PropertyDeclarationSyntax sourceNode,
            BasePropertyDeclarationSyntax targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context);
            if (!context.HasStatementLevelMutant)
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

            result = mutated.ReplaceNode(getter.Body!, SyntaxFactory.Block(
                    MutantPlacer.PlaceStatementControlledMutations(
                        getter.Body,
                        context.StatementLevelControlledMutations.Union(context.BlockLevelControlledMutations).Select(
                            m => (m.Id,
                                SyntaxFactory.ReturnStatement(
                                    sourceNode.ExpressionBody!.Expression
                                        .InjectMutation(m.Mutation)) as StatementSyntax)))))
                .WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken));
            context.BlockLevelControlledMutations.Clear();
            context.StatementLevelControlledMutations.Clear();
            return result;
        }
    }
}
