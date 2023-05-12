namespace Stryker.RegexMutators;
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
using RegexParser.Nodes.GroupNodes;

public class RegexMutantOrchestrator
{
    private readonly string _pattern;
    private readonly IDictionary<Type, IEnumerable<IRegexMutator>> _mutatorsByRegexNodeType;
    private RegexNode _root;

    public RegexMutantOrchestrator(string pattern)
    {
        _pattern = pattern;
        _mutatorsByRegexNodeType = new Dictionary<Type, IEnumerable<IRegexMutator>>
        {
            { typeof(AnchorNode), new List<IRegexMutator> { new AnchorRemovalMutator() } },
            { typeof(QuantifierNode), new List<IRegexMutator> { new QuantifierRemovalMutator() } },
            { typeof(CharacterClassNode), new List<IRegexMutator> { new CharacterClassNegationMutator() } },
            { typeof(CharacterClassShorthandNode), new List<IRegexMutator> { new CharacterClassShorthandNegationMutator() } },
            { typeof(QuantifierNOrMoreNode), new List<IRegexMutator> { new QuantifierUnlimitedQuantityMutator() } },
            { typeof(QuantifierNMNode), new List<IRegexMutator> { new QuantifierQuantityMutator() } },
            { typeof(LookaroundGroupNode), new List<IRegexMutator> { new LookAroundMutator() } },
        };
    }

    public IEnumerable<RegexMutation> Mutate()
    {
        try
        {
            var parser = new Parser(_pattern);
            RegexTree tree = parser.Parse();
            _root = tree.Root;
        }

        catch (RegexParseException)
        {
            yield break;
        }

        var regexNodes = _root.GetDescendantNodes().ToList();
        regexNodes.Add(_root);

        foreach (RegexMutation mutant in regexNodes.SelectMany(node => FindMutants(node, _root)))
        {
            yield return mutant;
        }
    }

    private IEnumerable<RegexMutation> FindMutants(RegexNode regexNode, RegexNode root)
    {
        return _mutatorsByRegexNodeType
            .Where(item => regexNode.GetType() == item.Key || regexNode.GetType().IsSubclassOf(item.Key))
            .SelectMany(item => item.Value)
            .SelectMany(mutator => mutator.Mutate(regexNode, root));
    }

}