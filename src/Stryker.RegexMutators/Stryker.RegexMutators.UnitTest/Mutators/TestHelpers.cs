using System.Collections.Generic;
using Stryker.Regex.Parser;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;
using System.Linq;

namespace Stryker.RegexMutators.UnitTest.Mutators;

public static class TestHelpers
{
    public static IEnumerable<RegexMutation> ParseAndMutate<T>(string pattern, RegexMutatorBase<T> mutator)
        where T : RegexNode
    {
        var root = new Parser(pattern).Parse().Root;
        IEnumerable<RegexNode> allNodes = [..root.GetDescendantNodes(), root];
        return allNodes.Where(((IRegexMutator)mutator).CanHandle).OfType<T>().SelectMany(node => mutator.ApplyMutations(node, root));
    }
}
