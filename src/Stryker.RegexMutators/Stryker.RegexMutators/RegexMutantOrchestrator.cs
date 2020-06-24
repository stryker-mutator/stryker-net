using RegexParser;
using RegexParser.Exceptions;
using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using RegexParser.Nodes.CharacterClass;
using RegexParser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators
{
    public class RegexMutantOrchestrator
    {
        private readonly string _pattern;
        private RegexNode _root;
        private IDictionary<Type, IEnumerable<IRegexMutator>> _mutatorsByRegexNodeType;

        public RegexMutantOrchestrator(string pattern)
        {
            _pattern = pattern;
        }

        public IEnumerable<RegexMutation> Mutate()
        {
            var parser = new Parser(_pattern);
            try
            {
                RegexTree tree = parser.Parse();
                _root = tree.Root;
            }
            catch (RegexParseException)
            {
                yield break;
            }

            _mutatorsByRegexNodeType = new Dictionary<Type, IEnumerable<IRegexMutator>>
            {
                {
                    typeof(AnchorNode),
                    new List<IRegexMutator>
                    {
                        new AnchorRemovalMutator(_root)
                    }
                },
                {
                    typeof(QuantifierNode),
                    new List<IRegexMutator>
                    {
                        new QuantifierRemovalMutator(_root)
                    }
                },
                {
                    typeof(CharacterClassNode),
                    new List<IRegexMutator>
                    {
                        new CharacterClassNegationMutator(_root)
                    }
                },
                {
                    typeof(CharacterClassShorthandNode),
                    new List<IRegexMutator>
                    {
                        new CharacterClassShorthandNegationMutator(_root)
                    }
                },
            };

            foreach (RegexMutation mutant in _root.GetDescendantNodes().SelectMany(FindMutants))
            {
                yield return mutant;
            }
            foreach (RegexMutation mutant in FindMutants(_root))
            {
                yield return mutant;
            }
        }

        private IEnumerable<RegexMutation> FindMutants(RegexNode regexNode)
        {
            return _mutatorsByRegexNodeType
                .Where(item => regexNode.GetType() == item.Key || regexNode.GetType().IsSubclassOf(item.Key))
                .SelectMany(item => item.Value)
                .SelectMany(mutator => mutator.Mutate(regexNode))
                .Select(mutant => mutant);
        }

    }
}