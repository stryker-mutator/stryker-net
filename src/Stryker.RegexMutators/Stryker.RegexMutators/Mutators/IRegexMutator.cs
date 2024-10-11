using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;

namespace Stryker.RegexMutators.Mutators;

public interface IRegexMutator
{
    IEnumerable<RegexMutation> Mutate(RegexNode node, RegexNode root);
}