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

        // mutations must be controlled at the statement level
        protected override ExpressionSyntax InjectMutations(InitializerExpressionSyntax originalNode, ExpressionSyntax mutatedNode,
            MutationContext context, IEnumerable<Mutant> mutations)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return mutatedNode;
        }

        public ArrayInitializerOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
