using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;

namespace Stryker.RegexMutators.Mutators;

public class CharacterClassShorthandNegationMutator : RegexMutatorBase<CharacterClassShorthandNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassShorthandNode node, RegexNode root)
    {
        yield return CharacterClassShorthandNegation(node, root);
    }

    private RegexMutation CharacterClassShorthandNegation(CharacterClassShorthandNode node, RegexNode root)
    {
        var negatedShorthandCharacter = char.IsLower(node.Shorthand) ? char.ToUpper(node.Shorthand) : char.ToLower(node.Shorthand);
        var replacementNode = new CharacterClassShorthandNode(negatedShorthandCharacter);

        var (Start, _) = node.GetSpan();
        return new RegexMutation
        {
            OriginalNode = node,
            ReplacementNode = replacementNode,
            DisplayName = "Regex character class shorthand negation mutation",
            Description = $"Character class shorthand \"{node}\" was replaced with \"{replacementNode}\" at offset {Start}.",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
