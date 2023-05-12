namespace Stryker.RegexMutators.Mutators;
using RegexParser.Nodes;
using System.Collections.Generic;
using RegexParser.Nodes.GroupNodes;

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
