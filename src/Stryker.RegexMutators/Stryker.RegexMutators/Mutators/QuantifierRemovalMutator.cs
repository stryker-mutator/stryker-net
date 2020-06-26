using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators
{
    public class QuantifierRemovalMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
    {
        public QuantifierRemovalMutator(RegexNode root)
            : base(root)
        {
        }

        public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNode node)
        {
            yield return QuantifierRemoval(node);
        }

        private RegexMutation QuantifierRemoval(QuantifierNode node)
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
                Description = $"Quantifier \"{Root.ToString().Substring(span.Start, length)}\" was removed at offset {span.Start}.",
                ReplacementPattern = Root.ReplaceNode(target, replacementNode).ToString()
            };
        }
    }
}
