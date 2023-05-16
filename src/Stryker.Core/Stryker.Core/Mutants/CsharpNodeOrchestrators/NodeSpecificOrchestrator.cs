using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal abstract class NodeOrchestratorBase
    {
        protected static readonly Regex _pattern = new("^\\s*\\/\\/\\s*Stryker", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(20));
        protected static readonly Regex _parser = new("^\\s*\\/\\/\\s*Stryker\\s*(disable|restore)\\s*(once|)\\s*([^:]*)\\s*:?(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(20));
        protected static readonly ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<NodeOrchestratorBase>();

        public static MutationContext ParseStrykerComment(MutationContext context, Match match, SyntaxNode node)
        {
            const int ModeGroup = 1;
            const int OnceGroup = 2;
            const int MutatorsGroup = 3;
            const int CommentGroup = 4;

            // get the ignore comment
            var comment = match.Groups[CommentGroup].Value.Trim();
            if (string.IsNullOrEmpty(comment))
            {
                comment = "Ignored via code comment.";
            }

            var disable = match.Groups[ModeGroup].Value.ToLower() switch
            {
                "disable" => true,
                _ => false,
            };

            Mutator[] filteredMutators;
            if (match.Groups[MutatorsGroup].Value.ToLower().Trim() == "all")
            {
                filteredMutators = Enum.GetValues<Mutator>();
            }
            else
            {
                var labels = match.Groups[MutatorsGroup].Value.ToLower().Split(',');
                filteredMutators = new Mutator[labels.Length];
                for (var i = 0; i < labels.Length; i++)
                {
                    if (Enum.TryParse<Mutator>(labels[i], true, out var value))
                    {
                        filteredMutators[i] = value;
                    }
                    else
                    {
                        _logger.LogError(
                            $"{labels[i]} not recognized as a mutator at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}. Legal values are {string.Join(',', Enum.GetValues<Mutator>())}.");
                    }
                }
            }

            return context.FilterMutators(disable, filteredMutators, match.Groups[OnceGroup].Value.ToLower() == "once", comment);
        }
    }

    /// <summary>
    /// This purpose of each implementation of this class is to support one specific C# code construct during the mutation process.
    /// Indeed some constructs need to be handled specifically to ensure successful mutations.
    /// Others are used to inject the need mutation control logic. It is strongly suggested to review each of those classes to
    /// get a grasp of how they work before adding a new one.
    /// </summary>
    /// <typeparam name="TNode">Roslyn type which represents the C# construct</typeparam>
    /// <typeparam name="TBase">Roslyn type which represents a generalization of this type</typeparam>
    /// <remarks>Those classes are an implementation of the 'Strategy' pattern. They must remain stateless, as the same instance is used for all syntax node of
    /// the given type. They can still embark some readonly options/parameters, as kong as they remain constant during parsing.</remarks>
    internal abstract class NodeSpecificOrchestrator<TNode, TBase> : NodeOrchestratorBase, INodeMutator where TBase : SyntaxNode where TNode : TBase
    {
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
        protected virtual IEnumerable<Mutant> GenerateMutationForNode(TNode node, MutationContext context) => context.GenerateMutantsForNode(node);

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
            MutationContext context) => context;

        /// <summary>
        /// Mutate children, grandchildren (recursively). 
        /// </summary>
        /// <param name="node">Node which children will be mutating</param>
        /// <param name="context">Mutation status</param>
        /// <returns>A <see cref="TBase"/> instance with the mutated children.</returns>
        /// <remarks>Override this method if you want to control how the node's children are mutated. simply return <see cref="node"/> if you want to
        /// skip mutation the children node.</remarks>
        protected virtual TBase OrchestrateChildrenMutation(TNode node, MutationContext context) =>
            node.ReplaceNodes(node.ChildNodes(),
                computeReplacementNode: (original, _) => MutateSingleNode(original, context));

        protected virtual SyntaxNode MutateSingleNode(SyntaxNode node, MutationContext context) => context.FindHandler(node).Mutate(node, context);

        protected virtual MutationContext PrepareContext(TNode node, MutationContext context)
        {
            foreach (var commentTrivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia)).Select(t => t.ToString()))
            {
                // perform a quick pattern check to see if it is a 'Stryker comment'
                if (!_pattern.Match(commentTrivia).Success)
                {
                    continue;
                }
                var match = _parser.Match(commentTrivia);
                if (match.Success)
                {
                    // this is a Stryker comments, now we parse it
                    context = ParseStrykerComment(context, match, node);
                    break;
                }

                _logger.LogWarning($"Invalid Stryker comments at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}.");
            }
            return context;
        }

        protected virtual void RestoreContext(MutationContext context) { }

        /// <summary>
        /// Mutates a node and its children. Update the mutation context with mutations needed to be injected in a higher level node.
        /// </summary>
        /// <param name="node">Node to be mutated</param>
        /// <param name="context">Mutation context</param>
        /// <returns>A <see cref="SyntaxNode"/> instance will all injected mutations.</returns>
        public virtual SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var specificNode = node as TNode;
            context = PrepareContext(specificNode, context);
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
