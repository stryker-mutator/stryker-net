using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;

namespace Stryker.RegexMutators.Mutators;

public sealed class CharacterClassToAnyCharMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
{
    private static CharacterClassNode AnyChar =>
        new(new CharacterClassCharacterSetNode([new CharacterClassShorthandNode('w'), new CharacterClassShorthandNode('W')]),
            false);

    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassNode node, RegexNode root)
    {
        var replaceNode = root.ReplaceNode(node, AnyChar);

        if (node.CharacterSet.ChildNodes is List<RegexNode> and
            [
                CharacterClassShorthandNode { Shorthand: 'w' or 'W' },
                CharacterClassShorthandNode { Shorthand: 'w' or 'W' }
            ] && node.Subtraction is null)
        {
            yield break;
        }

        yield return new RegexMutation
        {
            OriginalNode    = node,
            ReplacementNode = AnyChar,
            DisplayName     = """Regex character class to "[\w\W]" change""",
            Description =
                $"""Replaced regex node "{node}" with "[\w\W]" at offset {node.GetSpan().Start}.""",
            ReplacementPattern = replaceNode.ToString()
        };
    }
}
