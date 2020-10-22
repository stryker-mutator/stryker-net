using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators
{
    public class QuantifierRemovalMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
    {
        public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNode node, RegexNode root)
        {
            yield return QuantifierRemoval(node, root);
        }

        private RegexMutation QuantifierRemoval(QuantifierNode node, RegexNode root)
        {
            var replacementNode = node.ChildNodes.FirstOrDefault();
            var span = node.GetSpan();
            int length;
            RegexNode target;

            if (node.Parent is LazyNode)
            {
                target = node.Parent;
                length = span.Length + 1;
            }
            else
            {
                target = node;
                length = span.Length;
            }

            return new RegexMutation
            {
                OriginalNode = target,
                ReplacementNode = replacementNode,
                DisplayName = "Regex quantifier removal mutation",
                Description = $"Quantifier \"{root.ToString().Substring(span.Start, length)}\" was removed at offset {span.Start}.",
                ReplacementPattern = root.ReplaceNode(target, replacementNode).ToString()
            };
        }
    }
}
