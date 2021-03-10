using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class PostfixUnaryExpressionOrchestrator : ExpressionSpecificOrchestrator<PostfixUnaryExpressionSyntax>
    {
        protected override bool CanHandle(PostfixUnaryExpressionSyntax t)
        {
            return t.Parent is ExpressionStatementSyntax || t.Parent is ForStatementSyntax;
        }

        //even if they are 'expressions', the ++ and -- operators need to be controlled at the statement level.
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
