using RegexParser.Nodes;
using System.Collections.Generic;
using RegexParser.Nodes.GroupNodes;

namespace Stryker.RegexMutators.Mutators
{
    public class LookAroundMutator : RegexMutatorBase<LookaroundGroupNode>, IRegexMutator
    {
        public override IEnumerable<RegexMutation> ApplyMutations(LookaroundGroupNode node, RegexNode root)
        {
            yield return FlipLookAround(node, root, node.Lookahead, ! node.Possitive);
            yield return FlipLookAround(node, root, ! node.Lookahead, node.Possitive);
        }

        private RegexMutation FlipLookAround(LookaroundGroupNode node, RegexNode root, bool lookAhead, bool positive)
        {
            var replacementNode = new LookaroundGroupNode(lookAhead, positive, node.ChildNodes);

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
}
