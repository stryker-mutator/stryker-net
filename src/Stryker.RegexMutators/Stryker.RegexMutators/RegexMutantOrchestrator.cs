using RegexParser;
using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using RegexParser.Nodes.CharacterClass;
using RegexParser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators
{
    public class RegexMutantOrchestrator
    {
        private readonly string _pattern;
        private RegexNode _root;

        public RegexMutantOrchestrator(string pattern)
        {
            _pattern = pattern;
        }

        public IEnumerable<string> Mutate()
        {
            var parser = new Parser(_pattern);
            RegexTree tree = parser.Parse();
            _root = tree.Root;

            foreach (string mutant in _root.GetDescendantNodes().SelectMany(node => FindMutants(node)))
            {
                yield return mutant;
            }
            foreach (string mutant in FindMutants(_root))
            {
                yield return mutant;
            }
        }

        private IEnumerable<string> FindMutants(RegexNode regexNode)
        {
            if (regexNode is AnchorNode anchorNode)
            {
                foreach (string mutant in new AnchorRemovalMutator(_root).ApplyMutations(anchorNode))
                {
                    yield return mutant;
                }
            }

            else if (regexNode is QuantifierNode quantifierNode)
            {
                foreach (string mutant in new QuantifierRemovalMutator(_root).ApplyMutations(quantifierNode))
                {
                    yield return mutant;
                }
            }

            else if (regexNode is CharacterClassNode characterClassNode)
            {
                foreach (string mutant in new CharacterClassNegationMutator(_root).ApplyMutations(characterClassNode))
                {
                    yield return mutant;
                }
            }

            else if (regexNode is CharacterClassShorthandNode characterClassShorthandNode)
            {
                foreach (string mutant in new CharacterClassShorthandNegationMutator(_root).ApplyMutations(characterClassShorthandNode))
                {
                    yield return mutant;
                }
            }
        }

    }
}