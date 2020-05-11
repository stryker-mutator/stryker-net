using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public class AnchorRemovalMutator : RegexMutatorBase<AnchorNode>, IRegexMutator
    {
        private readonly RegexNode _root;

        public AnchorRemovalMutator(RegexNode root)
        {
            _root = root;
        }

        public override IEnumerable<string> ApplyMutations(AnchorNode node)
        {
            yield return AnchorRemoval(node);
        }

        private string AnchorRemoval(AnchorNode node)
        {
            return _root.RemoveNode(node).ToString();
        }
    }
}
