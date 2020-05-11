using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators
{
    public class QuantifierRemovalMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
    {
        private RegexNode Root { get; }

        public QuantifierRemovalMutator(RegexNode root)
        {
            Root = root;
        }

        public override IEnumerable<string> ApplyMutations(QuantifierNode node)
        {
            yield return QuantifierRemoval(node);
        }

        private string QuantifierRemoval(QuantifierNode node)
        {
            if (node.Parent is LazyNode)
            {
                return Root.ReplaceNode(node.Parent, node.ChildNodes.First()).ToString();
            }
            return Root.ReplaceNode(node, node.ChildNodes.First()).ToString();
        }
    }
}
