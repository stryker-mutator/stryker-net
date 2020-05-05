using RegexParser;
using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
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

            foreach (string m in _root.GetDescendantNodes().SelectMany(node => FindMutants(node)))
            {
                yield return m;
            }
            foreach (string m in FindMutants(_root))
            {
                yield return m;
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
        }

    }
}