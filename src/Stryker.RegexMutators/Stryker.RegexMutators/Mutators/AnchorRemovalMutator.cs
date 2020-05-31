using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public class AnchorRemovalMutator : RegexMutatorBase<AnchorNode>, IRegexMutator
    {
        public AnchorRemovalMutator(RegexNode root)
            : base(root)
        {
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
