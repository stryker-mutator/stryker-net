using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;

namespace Stryker.RegexMutators.Mutators;

public sealed class QuantifierReluctantAdditionMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
{
    /// <inheritdoc />
    bool IRegexMutator.CanHandle(RegexNode node) => node is QuantifierNode && node.Parent is not LazyNode;

    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNode node, RegexNode root)
    {
        var replacementNode = new LazyNode(node);

        yield return new RegexMutation
        {
            OriginalNode    = node,
            ReplacementNode = replacementNode,
            DisplayName     = "Regex greedy quantifier to reluctant quantifier modification",
            Description =
                $"""Quantifier "{node}" was replace with "{replacementNode}" at offset {node.GetSpan().Start}.""",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
