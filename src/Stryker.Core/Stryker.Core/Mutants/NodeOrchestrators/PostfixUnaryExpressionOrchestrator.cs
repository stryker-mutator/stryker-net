using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class PostfixUnaryExpressionOrchestrator: ExpressionSpecificOrchestrator<PostfixUnaryExpressionSyntax>
    {
        protected override bool CanHandle(PostfixUnaryExpressionSyntax t)
        {
            return t.Parent is ExpressionStatementSyntax || t.Parent is ForStatementSyntax;
        }

        public PostfixUnaryExpressionOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override ExpressionSyntax InjectMutations(PostfixUnaryExpressionSyntax originalNode, ExpressionSyntax mutatedNode,
            MutationContext context, IEnumerable<Mutant> mutations)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return mutatedNode;
        }
    }
}