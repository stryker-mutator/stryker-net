using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// General handler for Statements. Remember to inherit from this class when you wand to create a statement specific logic.
    /// </summary>
    /// <typeparam name="T">Statement syntax type. Must inherit from <see cref="StatementSyntax"/></typeparam>
    internal class StatementSpecificOrchestrator<T>: NodeSpecificOrchestrator<T, StatementSyntax> where T: StatementSyntax
    {
        private static Regex parser = new Regex("^\\s*\\/\\/\\s*Stryker\\s*(disable|restore)\\s*(\\w+)\\s*$", RegexOptions.Compiled);

        public StatementSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override MutationContext PrepareContext(MutationContext context)
        {
            context.Store.EnterStatement();
            return context.Clone();
        }

        protected override void RestoreContext(MutationContext context)
        {
            context.Store.LeaveStatement();
        }

        protected override StatementSyntax OrchestrateChildrenMutation(T node, MutationContext context)
        {
            foreach (var commentTrivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)).Select(t => t.ToString()))
            {
                var match = parser.Match(commentTrivia);
                if (match.Success && match.Groups[1].Value=="disable")
                {
                    // do not mutate this statement
                    return node;
                }
            }
            return base.OrchestrateChildrenMutation(node, context);
        }
        
        /// <inheritdoc/>
        /// <remarks>Inject pending mutations that are controlled with 'if' statements.</remarks>
        protected override StatementSyntax InjectMutations(T sourceNode, StatementSyntax targetNode, MutationContext context)
        {
            return context.Store.PlaceStatementMutations(targetNode,
                m => (sourceNode as StatementSyntax).InjectMutation(m));
        }

        /// <inheritdoc/>
        /// <remarks>Mutations are stored ath statement level.</remarks>
        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            if (mutations.Any())
            {
                context.Store.StoreMutations(mutations, MutationControl.Statement);
            }
            return context;
        }
    }
}
