using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// This purpose of ech implementation of this class is to support one specific C# code construct during the mutation process.
    /// Indeed some constructs need to be handled specifically to ensure successful mutations.
    /// Others are used to inject the need mutation control logic. It is strongly suggested to review each of those classes to
    /// get a grasp of how they work before adding a new one.
    /// </summary>
    /// <typeparam name="TNode">Roslyn type which represents the C# construct</typeparam>
    /// <typeparam name="TBase">Roslyn type which represents a generalization of this type</typeparam>
    internal abstract class NodeSpecificOrchestrator<TNode, TBase>:INodeMutator where TBase: SyntaxNode where TNode: TBase
    {
        protected CsharpMutantOrchestrator MutantOrchestrator;

        protected NodeSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator)
        {
            MutantOrchestrator = mutantOrchestrator;
        }

        /// Get the Roslyn type handled by this class
        /// </summary>
        public Type ManagedType => typeof(TNode);

        /// <summary>
        /// Checks if this class will manage a specific node.
        /// </summary>
        /// <param name="t">Syntax node to be tested</param>
        /// <returns>True if this class can process the provided node.</returns>
        /// <remarks>Default implementation always returns true. You can override this method to have several classes supporting various sub cases for a single node type.</remarks>
        protected virtual bool CanHandle(TNode t) => t!=null;

        /// <summary>
        /// Checks if this class will manage a specific node.
        /// </summary>
        /// <param name="t">Syntax node to be tested</param>
        /// <returns>True if this class can process the provided node.</returns>
        /// <remarks>Delegate the implementation to an polymorphic implementation.</remarks>
        public bool CanHandle(SyntaxNode t) => CanHandle(t as TNode);

        /// <summary>
        /// Inject mutation(s) in this node.
        /// </summary>
        /// <param name="sourceNode">Original, unmodified syntax node</param>
        /// <param name="targetNode">Variant of <paramref name="sourceNode"/> including mutated children.</param>
        /// <param name="context">Mutation context which contains pending mutations.</param>
        /// <returns>A syntax node (typeof <see cref="TBase"></see>) with mutations injected, if possible./></returns>
        /// <remarks>Override this method when you need to inject some code (e.g : mutation control, or analysis markers).</remarks>
        protected virtual TBase InjectMutations(TNode sourceNode, TBase targetNode, MutationContext context) => targetNode;

        /// <summary>
        /// Generates and returns the list of possible mutations for the provided node.
        /// </summary>
        /// <param name="node">Node to generate mutations from.</param>
        /// <param name="context">Mutation context.</param>
        /// <returns>A list of <see cref="Mutant"/>s for the given node.</returns>
        /// <remarks>You should not override this, unless you want to block mutation generation for the node. Then returns and empty list.</remarks>
        protected virtual IEnumerable<Mutant> GenerateMutationForNode(TNode node, MutationContext context) => MutantOrchestrator.GenerateMutationsForNode(node, context);

        /// <summary>
        /// Stores provided mutations.
        /// </summary>
        /// <param name="node">Associated node.</param>
        /// <param name="mutations">Mutations to store</param>
        /// <param name="context">Mutation context.</param>
        /// <returns>A <see cref="MutationContext"/>instance storing existing mutations as well as the one provided</returns>
        /// <remarks>You need to override this method if you need to ensure mutations are controlled at with 'if' at the statement or block level.</remarks>
        protected virtual MutationContext StoreMutations(TNode node,
            IEnumerable<Mutant> mutations,
            MutationContext context) =>  context;

        /// <summary>
        /// Mutate children, grandchildren (recursively). 
        /// </summary>
        /// <param name="node">Node which children will be mutating</param>
        /// <param name="context">Mutation status</param>
        /// <returns>A <see cref="TBase"/> instance with the mutated children.</returns>
        /// <remarks>Override this method if you want to control how the node's children are mutated. simply return <see cref="node"/> if you want to
        /// skip mutation the children node.</remarks>
        protected virtual TBase OrchestrateChildrenMutation(TNode node, MutationContext context)
        {
            return node.ReplaceNodes(node.ChildNodes(),
                computeReplacementNode: (original, _) => MutateSingleNode(original, context));
        }

        protected virtual SyntaxNode MutateSingleNode(SyntaxNode node, MutationContext context)
        {
            if (!SyntaxHelper.CanBeMutated(node))
            {
                return node;
            }

            var handler = MutantOrchestrator.GetHandler(node);
            return handler.Mutate(node, context);
        }

        protected virtual MutationContext PrepareContext(MutationContext context) => context.Clone();

        protected virtual void RestoreContext(MutationContext context)
        {}

        /// <summary>
        /// Mutates a node and its children. Update the mutation context with mutations needed to be injected in a higher level node.
        /// </summary>
        /// <param name="node">Node to be mutated</param>
        /// <param name="context">Mutation context</param>
        /// <returns>A <see cref="SyntaxNode"/> instance will all injected mutations.</returns>
        public SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var specificNode = node as TNode;
            context = PrepareContext(context);
            // we generate mutations for this node (to help numbering being in 'code reading' order)
            var mutations = GenerateMutationForNode(specificNode, context);
            SyntaxNode result = InjectMutations(specificNode,
                OrchestrateChildrenMutation(specificNode, context),
                StoreMutations(specificNode, mutations, context));

            RestoreContext(context);
            return result;
        }
    }
}
