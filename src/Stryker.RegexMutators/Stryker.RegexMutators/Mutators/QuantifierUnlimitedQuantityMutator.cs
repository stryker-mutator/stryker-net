using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators;

public class QuantifierUnlimitedQuantityMutator : RegexMutatorBase<QuantifierNOrMoreNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNOrMoreNode node, RegexNode root)
    {
        if (node.N - 1 >= 0)
        {
            yield return QuantityVariation(node, root, node.N - 1);
        }

        yield return QuantityVariation(node, root, node.N + 1);
    }

    private RegexMutation QuantityVariation(QuantifierNOrMoreNode node, RegexNode root, int from)
    {
        var replacementNode = new QuantifierNOrMoreNode(from, node.ChildNodes.FirstOrDefault());

        return new RegexMutation
        {
            OriginalNode = node,
            ReplacementNode = replacementNode,
            DisplayName = "Regex greedy quantifier quantity mutation",
            Description = $"Quantifier \"{node}\" was replaced with \"{replacementNode}\" at offset {node.GetSpan().Start}.",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
