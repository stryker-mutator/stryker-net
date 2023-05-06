using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators;

public class AnchorRemovalMutator : RegexMutatorBase<AnchorNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(AnchorNode node, RegexNode root)
    {
        yield return AnchorRemoval(node, root);
    }

    private RegexMutation AnchorRemoval(AnchorNode node, RegexNode root)
    {
        var span = node.GetSpan();
        return new RegexMutation
        {
            OriginalNode = node,
            DisplayName = "Regex anchor removal mutation",
            Description = $"Anchor \"{root.ToString().Substring(span.Start, span.Length)}\" was removed at offset {span.Start}.",
            ReplacementPattern = root.RemoveNode(node).ToString()
        };
    }
}
