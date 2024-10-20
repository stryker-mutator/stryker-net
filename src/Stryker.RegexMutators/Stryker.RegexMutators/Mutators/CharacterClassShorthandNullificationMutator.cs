using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;

namespace Stryker.RegexMutators.Mutators;

public sealed class CharacterClassShorthandNullificationMutator : RegexMutatorBase<CharacterClassShorthandNode>, IRegexMutator
{
    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassShorthandNode node, RegexNode root)
    {
        var replacementNode = new CharacterNode(node.Shorthand);

        yield return new RegexMutation
        {
            OriginalNode    = node,
            ReplacementNode = replacementNode,
            DisplayName     = "Regex predefined character class nullification",
            Description =
                $"""Character class shorthand "{node}" was replaced with "{replacementNode}" at offset {node.GetSpan().Start}.""",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
