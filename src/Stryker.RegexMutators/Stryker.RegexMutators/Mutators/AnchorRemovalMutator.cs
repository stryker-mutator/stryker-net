using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.AnchorNodes;

namespace Stryker.RegexMutators.Mutators;

public class AnchorRemovalMutator : RegexMutatorBase<AnchorNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(AnchorNode node, RegexNode root)
    {
        yield return AnchorRemoval(node, root);
    }

    private RegexMutation AnchorRemoval(AnchorNode node, RegexNode root)
    {
        var (Start, Length) = node.GetSpan();
        return new RegexMutation
        {
            OriginalNode = node,
            DisplayName = "Regex anchor removal mutation",
            Description = $"Anchor \"{root.ToString().Substring(Start, Length)}\" was removed at offset {Start}.",
            ReplacementPattern = root.RemoveNode(node).ToString()
        };
    }
}
