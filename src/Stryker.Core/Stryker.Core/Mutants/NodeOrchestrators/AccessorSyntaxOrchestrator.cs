using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class AccessorSyntaxOrchestrator: NodeSpecificOrchestrator<AccessorDeclarationSyntax, SyntaxNode>
    {
        public AccessorSyntaxOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {}

        protected override SyntaxNode InjectMutations(AccessorDeclarationSyntax sourceNode, SyntaxNode targetNode, MutationContext context)
        {
            var result = base.InjectMutations(sourceNode, targetNode, context) as AccessorDeclarationSyntax;
            if (result?.Body == null && result?.ExpressionBody == null)
            {
                return result;
            }

            if (!context.HasStatementLevelMutant)
            {
                return result;
            }

            if (result.Body == null)
            {
                result = MutantPlacer.ConvertExpressionToBody(result);
            }

            return result.WithBody(SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(result.Body,sourceNode.ExpressionBody!.Expression, sourceNode.NeedsReturn())));
        }
    }
}
