using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;

namespace Stryker.RegexMutators.Mutators;

public class CharacterClassNegationMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassNode node, RegexNode root)
    {
        yield return CharacterClassNegation(node, root);
    }

    private RegexMutation CharacterClassNegation(CharacterClassNode node, RegexNode root)
    {
        var (Start, _) = node.GetSpan();
        var replacementNode = node.Subtraction == null ? new CharacterClassNode(node.CharacterSet, !node.Negated) : new CharacterClassNode(node.CharacterSet, node.Subtraction, !node.Negated);
        return new RegexMutation
        {
            OriginalNode = node,
            ReplacementNode = replacementNode,
            DisplayName = "Regex character class negation mutation",
            Description = $"Character class \"{node}\" was replaced with \"{replacementNode}\" at offset {Start}.",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
