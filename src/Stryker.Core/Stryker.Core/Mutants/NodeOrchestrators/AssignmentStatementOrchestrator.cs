using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class AssignmentStatementOrchestrator : ExpressionSpecificOrchestrator<AssignmentExpressionSyntax>
    {
        public AssignmentStatementOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        // mutations must be controlled at the statement level
        protected override MutationContext StoreMutations(IEnumerable<Mutant> mutations,
            AssignmentExpressionSyntax node,
            MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }
    }
}