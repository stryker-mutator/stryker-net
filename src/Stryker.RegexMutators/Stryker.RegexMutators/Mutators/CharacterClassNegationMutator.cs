using RegexParser.Nodes;
using RegexParser.Nodes.CharacterClass;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public class CharacterClassNegationMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
    {
        public CharacterClassNegationMutator(RegexNode root)
        {
            Root = root;
        }

        public override IEnumerable<string> ApplyMutations(CharacterClassNode node)
        {
            yield return CharacterClassNegation(node);
        }

        private string CharacterClassNegation(CharacterClassNode node)
        {
            var negatedCharacterClass = new CharacterClassNode(node.CharacterSet, !node.Negated);
            return Root.ReplaceNode(node, negatedCharacterClass).ToString();
        }
    }
}
