namespace Stryker.RegexMutators.Mutators;
using RegexParser.Nodes;
using RegexParser.Nodes.CharacterClass;
using System.Collections.Generic;

public class CharacterClassNegationMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassNode node, RegexNode root)
    {
        yield return CharacterClassNegation(node, root);
    }

    private RegexMutation CharacterClassNegation(CharacterClassNode node, RegexNode root)
    {
        var span = node.GetSpan();
        var replacementNode = node.Subtraction == null ? new CharacterClassNode(node.CharacterSet, !node.Negated) : new CharacterClassNode(node.CharacterSet, node.Subtraction, !node.Negated);
        return new RegexMutation
        {
            OriginalNode = node,
            ReplacementNode = replacementNode,
            DisplayName = "Regex character class negation mutation",
            Description = $"Character class \"{node}\" was replaced with \"{replacementNode}\" at offset {span.Start}.",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
