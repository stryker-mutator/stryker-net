using System.Collections.Generic;
using System.Linq;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;

namespace Stryker.RegexMutators.Mutators;

public class QuantifierRemovalMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNode node, RegexNode root)
    {
        yield return QuantifierRemoval(node, root);
    }

    private RegexMutation QuantifierRemoval(QuantifierNode node, RegexNode root)
    {
        var replacementNode = node.ChildNodes.FirstOrDefault();
        var (Start, Length) = node.GetSpan();
        int length;
        RegexNode target;

        if (node.Parent is LazyNode)
        {
            target = node.Parent;
            length = Length + 1;
        }
        else
        {
            target = node;
            length = Length;
        }

        return new RegexMutation
        {
            OriginalNode = target,
            ReplacementNode = replacementNode,
            DisplayName = "Regex quantifier removal mutation",
            Description = $"Quantifier \"{root.ToString().Substring(Start, length)}\" was removed at offset {Start}.",
            ReplacementPattern = root.ReplaceNode(target, replacementNode).ToString()
        };
    }
}
