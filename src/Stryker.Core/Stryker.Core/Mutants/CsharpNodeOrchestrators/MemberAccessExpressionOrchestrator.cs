using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class MemberAccessExpressionOrchestrator<T> : NodeSpecificOrchestrator<T, ExpressionSyntax> where T : ExpressionSyntax
    {
        private readonly Func<T, bool> _predicate;

        /// <summary>
        /// Builds a MemberAccessExpressionOrchestrator instance
        /// </summary>
        /// <param name="predicate">optional predicate to control which nodes can be orchestrated</param>
        public MemberAccessExpressionOrchestrator(Func<T, bool> predicate = null) => _predicate = predicate;

        protected override bool CanHandle(T t) => _predicate == null || _predicate(t);

        protected override ExpressionSyntax InjectMutations(T sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectMutations(targetNode, sourceNode);

        protected override MutationContext PrepareContext(T node, MutationContext context) =>
            // we are at expression level, except if we are explicitly already in a member access chain
            base.PrepareContext(node, context.Enter(context.CurrentControl != MutationControl.MemberAccess ? MutationControl.Expression : MutationControl.MemberAccess));

        protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());

    }
}
