using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutations for assignment. Its purpose is to ensure those mutations are controlled at statement level
/// </summary>
internal class AssignmentStatementOrchestrator : ExpressionSpecificOrchestrator<AssignmentExpressionSyntax>
{
    // mutations must be controlled at the statement level
    protected override MutationContext StoreMutations(AssignmentExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        context.AddStatementLevel(mutations);
        return context;
    }
}
