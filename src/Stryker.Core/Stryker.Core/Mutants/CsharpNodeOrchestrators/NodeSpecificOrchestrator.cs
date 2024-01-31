using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// This purpose of each implementation of this class is to support one specific C# code construct during the mutation process.
/// Indeed, some constructs need to be handled specifically to ensure successful mutations.
/// Others are used to inject the need mutation control logic. It is strongly suggested to review each of those classes to
/// get a grasp of how they work before adding a new one.
/// </summary>
/// <typeparam name="TNode">Roslyn type which represents the C# construct</typeparam>
/// <typeparam name="TBase">Type of the node once mutated. In practice, either <see cref="TNode"/> or a base class of it.</typeparam>
/// <remarks>Those classes are an implementation of the 'Strategy' pattern. They must remain stateless, as the same instance is used for all syntax node of
/// the given type. They can still embark some readonly options/parameters, as long as they remain constant during parsing.</remarks>
internal class NodeSpecificOrchestrator<TNode, TBase> : INodeOrchestrator where TBase : SyntaxNode where TNode : TBase{
    /// <summary>
    /// Get the Roslyn type handled by this class
    /// </summary>
    public Type ManagedType => typeof(TNode);

    /// <summary>
    /// Checks if this class will manage a specific node.
    /// </summary>
    /// <param name="t">Syntax node to be tested</param>
    /// <returns>True if this class can process the provided node.</returns>
    /// <remarks>Default implementation always returns true. You can override this method to have several classes supporting various sub cases for a single node type.</remarks>
    protected virtual bool CanHandle(TNode t) => t != null;

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
    /// <param name="semanticModel"></param>
    /// <param name="context">Mutation context which contains pending mutations.</param>
    /// <returns>A syntax node (typeof <see cref="TBase"></see>) with mutations injected, if possible./></returns>
    /// <remarks>Override this method when you need to inject some code (e.g : mutation control, or analysis markers).</remarks>
    protected virtual TBase InjectMutations(TNode sourceNode, TBase targetNode, SemanticModel semanticModel, MutationContext context) => targetNode;

    /// <summary>
    /// Generates and returns the list of possible mutations for the provided node.
    /// </summary>
    /// <param name="node">Node to generate mutations from.</param>
    /// <param name="semanticModel"></param>
    /// <param name="context">Mutation context.</param>
    /// <returns>A list of <see cref="Mutant"/>s for the given node.</returns>
    /// <remarks>You should not override this, unless you want to block mutation generation for the node. Then returns and empty list.</remarks>
    protected virtual IEnumerable<Mutant> GenerateMutationForNode(TNode node, SemanticModel semanticModel, MutationContext context) => context.GenerateMutantsForNode(node, semanticModel);

    /// <summary>
    /// Stores provided mutations.
    /// </summary>
    /// <param name="node">Associated node.</param>
    /// <param name="mutations">Mutations to store</param>
    /// <param name="context">Mutation context.</param>
    /// <returns>A <see cref="MutationContext"/>instance storing existing mutations as well as the one provided</returns>
    /// <remarks>You need to override this method if the generated mutations cannot be injected in place (via a conditional operator) but must be controlled
    /// at the statement or block level. Default implementation does nothing.</remarks>
    protected virtual MutationContext StoreMutations(TNode node, IEnumerable<Mutant> mutations, MutationContext context) => context.AddMutations(mutations);

    /// <summary>
    /// Mutate children, grandchildren (recursively). 
    /// </summary>
    /// <param name="node">Node which children will be mutating</param>
    /// <param name="semanticModel"></param>
    /// <param name="context">Mutation status</param>
    /// <returns>A <see cref="TBase"/> instance with the mutated children.</returns>
    /// <remarks>Override this method if you want to control how the node's children are mutated. Simply return <see cref="node"/> if you want to
    /// skip mutating the children node.</remarks>
    protected virtual TBase OrchestrateChildrenMutation(TNode node, SemanticModel semanticModel, MutationContext context) =>
        node.ReplaceNodes(node.ChildNodes(),
            computeReplacementNode: (original, _) => context.FindHandler(original).Mutate(original, semanticModel, context));

    /// <summary>
    /// Set up the mutation context before triggering mutation.
    /// </summary>
    /// <param name="node">Node of interest</param>
    /// <param name="context">context to be updated</param>
    /// <returns>a context capturing changes, if any</returns>
    /// <remarks>base implementation parse stryker comments.</remarks>
    protected virtual MutationContext PrepareContext(TNode node, MutationContext context) => CommentParser.ParseNodeComments(node, context);

    /// <summary>
    /// Restore the mutation context after mutation have been performed
    /// </summary>
    /// <param name="context">Context to be updated</param>
    /// <remarks>base implementation does nothing</remarks>
    protected virtual void RestoreContext(MutationContext context) { }

    /// <summary>
    /// Mutates a node and its children. Update the mutation context with mutations needed to be injected in a higher level node.
    /// The workflow is:
    /// 1) adjust the context
    /// 2) generate mutations for the node
    /// 3) store generated mutations in the context
    /// 4) recursively mutate children
    /// 5) (try to) inject mutations in this node
    /// 6) restore the context
    /// 7) return the mutated node (with mutated children)
    /// </summary>
    /// <param name="node">Node to be mutated</param>
    /// <param name="semanticModel"></param>
    /// <param name="context">Mutation context</param>
    /// <returns>A <see cref="SyntaxNode"/> instance will all injected mutations.</returns>
    public virtual SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context)
    {
        var specificNode = node as TNode;
        context = PrepareContext(specificNode, context);

        // we generate mutations for this node (to help numbering being in 'code reading' order)
        var mutations = GenerateMutationForNode(specificNode, semanticModel, context);
        SyntaxNode result = InjectMutations(specificNode,
            OrchestrateChildrenMutation(specificNode, semanticModel, context),
            semanticModel,
            StoreMutations(specificNode, mutations, context));

        RestoreContext(context);
        return result;
    }
}
