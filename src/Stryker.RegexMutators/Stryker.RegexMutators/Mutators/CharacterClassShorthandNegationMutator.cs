using RegexParser.Nodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public class CharacterClassShorthandNegationMutator : RegexMutatorBase<CharacterClassShorthandNode>, IRegexMutator
    {
        public CharacterClassShorthandNegationMutator(RegexNode root)
            : base(root)
        {
        }

        public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassShorthandNode node)
        {
            yield return CharacterClassShorthandNegation(node);
        }

        private RegexMutation CharacterClassShorthandNegation(CharacterClassShorthandNode node)
        {
            var negatedShorthandCharacter = char.IsLower(node.Shorthand) ? char.ToUpper(node.Shorthand) : char.ToLower(node.Shorthand);
            var replacementNode = new CharacterClassShorthandNode(negatedShorthandCharacter);
            var (start, _) = node.GetSpan();
            return new RegexMutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Regex character class shorthand negation mutation",
                Description = $"Character class shorthand \"{node}\" was replaced with \"{replacementNode}\" at offset {start}.",
                Pattern = Root.ReplaceNode(node, replacementNode).ToString()
            };
        }
    }
}
