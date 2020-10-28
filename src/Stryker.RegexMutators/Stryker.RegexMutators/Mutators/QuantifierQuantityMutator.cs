using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators
{
    public class QuantifierQuantityMutator : RegexMutatorBase<QuantifierNMNode>, IRegexMutator
    {
        public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNMNode node, RegexNode root)
        {
            var quantityVariations = new List<(int, int)>
            {
                (node.N - 1, node.M),
                (node.N + 1, node.M),
                (node.N, node.M - 1),
                (node.N, node.M + 1)
            };

            foreach (var (from, to) in quantityVariations)
            {
                if (from < 0 || to < 0 || from > to)
                {
                    continue;
                }

                yield return QuantityVariation(node, root, from, to);
            }
        }

        private RegexMutation QuantityVariation(QuantifierNMNode node, RegexNode root, int from, int to)
        {
            var replacementNode = new QuantifierNMNode(from, to, node.ChildNodes.FirstOrDefault());

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
