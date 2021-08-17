using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class LocalFunctionStatementOrchestrator : NodeSpecificOrchestrator<LocalFunctionStatementSyntax, LocalFunctionStatementSyntax>
    {
        public LocalFunctionStatementOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override LocalFunctionStatementSyntax InjectMutations(LocalFunctionStatementSyntax sourceNode, LocalFunctionStatementSyntax targetNode,
            MutationContext context)
        {
            // find out parameters
            targetNode = base.InjectMutations(sourceNode, targetNode, context);
            if (targetNode.Body != null)
            {
                // inject initialization to default for all out parameters
                targetNode = sourceNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Body,
                    sourceNode.ParameterList.Parameters.Where(p =>
                        p.Modifiers.Any(m => m.Kind() == SyntaxKind.OutKeyword))));
                // add a return in case we changed the control flow
                return MutantPlacer.AddEndingReturn(targetNode);
            }

            if (!context.HasStatementLevelMutant)
            {
                return targetNode;
            }

            // we need to move to a body version of the method
            targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);

            StatementSyntax mutatedBlock = targetNode.Body;

            var converter = targetNode.NeedsReturn()
                ? (Func<Mutation, StatementSyntax>) ((toConvert) =>
                    SyntaxFactory.ReturnStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert)))
                : (toConvert) =>
                    SyntaxFactory.ExpressionStatement(sourceNode.ExpressionBody!.Expression.InjectMutation(toConvert));

            mutatedBlock =
                MutantPlacer.PlaceStatementControlledMutations(mutatedBlock,
                    context.StatementLevelControlledMutations.Union(context.BlockLevelControlledMutations).
                        Select( m => (m.Id, converter(m.Mutation))));
            context.BlockLevelControlledMutations.Clear();
            context.StatementLevelControlledMutations.Clear();
            return targetNode.ReplaceNode(targetNode.Body!,
                SyntaxFactory.Block(mutatedBlock));
        }
    }
}
