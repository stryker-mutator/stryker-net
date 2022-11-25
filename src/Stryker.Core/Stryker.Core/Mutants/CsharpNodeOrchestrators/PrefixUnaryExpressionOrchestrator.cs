using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class PrefixUnaryExpressionOrchestrator : ExpressionSpecificOrchestrator<PrefixUnaryExpressionSyntax>
{
    protected override bool CanHandle(PrefixUnaryExpressionSyntax t) => t.Parent is ExpressionStatementSyntax or ForStatementSyntax;

    // even if they are 'expressions', the ++ and -- operators need to be controlled at the statement level.
    protected override MutationContext StoreMutations(PrefixUnaryExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        context.AddStatementLevel(mutations);
        return context;
    }
}
