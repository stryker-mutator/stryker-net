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

        public override IEnumerable<string> ApplyMutations(CharacterClassShorthandNode node)
        {
            yield return CharacterClassShorthandNegation(node);
        }

        private string CharacterClassShorthandNegation(CharacterClassShorthandNode node)
        {
            var negatedShorthand = char.IsLower(node.Shorthand) ? char.ToUpper(node.Shorthand) : char.ToLower(node.Shorthand);
            return Root.ReplaceNode(node, new CharacterClassShorthandNode(negatedShorthand)).ToString();
        }
    }
}
