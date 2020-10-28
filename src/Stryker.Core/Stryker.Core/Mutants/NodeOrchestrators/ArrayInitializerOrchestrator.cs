using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ArrayInitializerOrchestrator : ExpressionSpecificOrchestrator<InitializerExpressionSyntax>
    {
        protected override bool CanHandle(InitializerExpressionSyntax t)
        {
            return (t.Kind() == SyntaxKind.ArrayInitializerExpression && t.Expressions.Count > 0);
        }

        // mutations must be controlled at the statement level as those are not really expressions.
        protected override MutationContext StoreMutations(InitializerExpressionSyntax node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }

        public ArrayInitializerOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        { }
    }
}
