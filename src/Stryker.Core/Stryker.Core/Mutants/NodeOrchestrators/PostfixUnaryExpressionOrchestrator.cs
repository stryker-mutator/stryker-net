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

        // even if they are 'expressions', the ++ and -- operators need to be controlled at the statement level.
        protected override MutationContext StoreMutations(PostfixUnaryExpressionSyntax node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }

        public PostfixUnaryExpressionOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}