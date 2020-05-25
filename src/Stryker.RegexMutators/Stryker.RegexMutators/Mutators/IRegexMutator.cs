using RegexParser.Nodes;
using System.Collections.Generic;

namespace Stryker.RegexMutators.Mutators
{
    public interface IRegexMutator
    {
        IEnumerable<string> Mutate(RegexNode node);
    }
}