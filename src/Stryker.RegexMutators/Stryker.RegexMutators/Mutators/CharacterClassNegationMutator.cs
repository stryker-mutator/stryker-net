using RegexParser.Nodes;
using RegexParser.Nodes.CharacterClass;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public class CharacterClassNegationMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
    {
        public CharacterClassNegationMutator(RegexNode root)
            : base(root)
        {
        }

        public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassNode node)
        {
            yield return CharacterClassNegation(node);
        }

        private RegexMutation CharacterClassNegation(CharacterClassNode node)
        {
            var (start, _) = node.GetSpan();
            var replacementNode = node.Subtraction == null ?  new CharacterClassNode(node.CharacterSet, !node.Negated) : new CharacterClassNode(node.CharacterSet, node.Subtraction, !node.Negated);
            return new RegexMutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Regex character class negation mutation",
                Description = $"Character class \"{node}\" was replaced with \"{replacementNode}\" at offset {start}.",
                Pattern = Root.ReplaceNode(node, replacementNode).ToString()
            };
        }
    }
}
