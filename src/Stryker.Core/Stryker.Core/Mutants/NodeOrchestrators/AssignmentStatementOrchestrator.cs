using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class AssignmentStatementOrchestrator : ExpressionSpecificOrchestrator<AssignmentExpressionSyntax>
    {
        public AssignmentStatementOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        // mutations must be controlled at the statement level
        protected override MutationContext StoreMutations(AssignmentExpressionSyntax node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.Store.StoreMutations(mutations, MutationControl.Statement);
            return context;
        }
    }
}
