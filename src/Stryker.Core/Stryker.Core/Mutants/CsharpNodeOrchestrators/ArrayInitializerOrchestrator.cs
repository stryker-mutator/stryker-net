using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutations for array initializer. Its purpose is to ensure those mutations are controlled at statement level
/// </summary>
internal class ArrayInitializerOrchestrator : ExpressionSpecificOrchestrator<InitializerExpressionSyntax>
{
    protected override bool CanHandle(InitializerExpressionSyntax t) => t.Kind() == SyntaxKind.ArrayInitializerExpression && t.Expressions.Count > 0;

    // mutations must be controlled at the statement level as those are not really expressions.
    protected override MutationContext StoreMutations(InitializerExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        context.AddStatementLevel(mutations);
        return context;
    }
}
