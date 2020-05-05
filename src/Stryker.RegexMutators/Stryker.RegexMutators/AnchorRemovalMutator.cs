using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators
{
    public class AnchorRemovalMutator : RegexMutatorBase<AnchorNode>, IRegexMutator
    {
        private RegexNode Root { get; }

        public AnchorRemovalMutator(RegexNode root)
        {
            Root = root;
        }

        public override IEnumerable<string> ApplyMutations(AnchorNode node)
        {
            yield return AnchorRemoval(node);
        }

        private string AnchorRemoval(AnchorNode node)
        {
            return Root.RemoveNode(node).ToString();
        }
    }
}
