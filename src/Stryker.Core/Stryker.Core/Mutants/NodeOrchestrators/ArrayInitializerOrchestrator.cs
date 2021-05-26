using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            context.Store.StoreMutations(mutations, MutationControl.Statement);
            return context;
        }

        public ArrayInitializerOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {}
    }
}
