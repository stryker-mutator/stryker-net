using System.Collections.Generic;
using Stryker.Regex.Parser;
using Stryker.Regex.Parser.Exceptions;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators;

public sealed class RegexMutantOrchestrator(string pattern)
{
    private static readonly IReadOnlyCollection<IRegexMutator> _mutators =
    [
        // Level 1
        new AnchorRemovalMutator(),
        new CharacterClassNegationMutator(),
        new CharacterClassShorthandNegationMutator(),
        new QuantifierRemovalMutator(),
        new UnicodeCharClassNegationMutator(),
        new LookAroundMutator(),

        // Level 2
        new CharacterClassChildRemovalMutator(),
        new CharacterClassToAnyCharMutator(),
        new CharacterClassShorthandNullificationMutator(),
        new CharacterClassShorthandAnyCharMutator(),
        new QuantifierUnlimitedQuantityMutator(),
        new QuantifierQuantityMutator(),
        new QuantifierShortMutator(),
        new GroupToNcGroupMutator(),

        // Level 3
        new CharacterClassRangeMutator(),
        new QuantifierReluctantAdditionMutator()
    ];

    public IEnumerable<RegexMutation> Mutate()
    {
        RegexNode root;

        try
        {
            var parser = new Parser(pattern);
            var tree = parser.Parse();
            root = tree.Root;
        }
        catch (RegexParseException)
        {
            yield break;
        }

        IEnumerable<RegexNode> regexNodes = [..root.GetDescendantNodes(), root];

        foreach (var regexNode in regexNodes)
        {
            foreach (var item in _mutators)
            {
                if (item.CanHandle(regexNode))
                {
                    foreach (var regexMutation in item.Mutate(regexNode, root))
                    {
                        yield return regexMutation;
                    }
                }
            }
        }
    }
}
