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

        protected override MutationContext StoreMutations(IEnumerable<Mutant> mutations,
            PostfixUnaryExpressionSyntax node, MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }

        public PostfixUnaryExpressionOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}