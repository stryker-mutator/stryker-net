using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ArrayInitializerOrchestrator : NodeSpecificOrchestrator<InitializerExpressionSyntax>
    {
        protected override bool CanHandleThis(InitializerExpressionSyntax t)
        {
            return (t.Kind() == SyntaxKind.ArrayInitializerExpression && t.Expressions.Count > 0);
        }

        internal override SyntaxNode OrchestrateMutation(InitializerExpressionSyntax node, MutationContext context)
        {
            context.GenerateStatementLevelControlledMutants(node);
            return context.MutateChildren(node);
        }
    }
}
