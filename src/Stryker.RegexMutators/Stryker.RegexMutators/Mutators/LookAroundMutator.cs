using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.GroupNodes;

namespace Stryker.RegexMutators.Mutators;

public class LookAroundMutator : RegexMutatorBase<LookaroundGroupNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(LookaroundGroupNode node, RegexNode root)
    {
        var replacementNode = new LookaroundGroupNode(node.Lookahead, !node.Possitive, node.ChildNodes);

        yield return new RegexMutation
        {
            OriginalNode = node,
            ReplacementNode = replacementNode,
            DisplayName = "Regex greedy quantifier quantity mutation",
            Description = $"Quantifier \"{node}\" was replaced with \"{replacementNode}\" at offset {node.GetSpan().Start}.",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
