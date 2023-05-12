namespace Stryker.RegexMutators.Mutators;
using RegexParser.Nodes;
using System.Collections.Generic;

public interface IRegexMutator
{
    IEnumerable<RegexMutation> Mutate(RegexNode node, RegexNode root);
}