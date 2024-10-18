using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;

namespace Stryker.RegexMutators.Mutators;

public sealed class UnicodeCharClassNegationMutator : RegexMutatorBase<UnicodeCategoryNode>, IRegexMutator
{
    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(UnicodeCategoryNode node, RegexNode root)
    {
        var replacementNode = new UnicodeCategoryNode(node.Category, !node.Negated);

        yield return new RegexMutation
        {
            OriginalNode    = node,
            ReplacementNode = replacementNode,
            DisplayName     = "Regex Unicode character class negation mutation",
            Description =
                $"""Unicode category "{node}" was replaced with "{replacementNode}" at offset {node.GetSpan().Start}.""",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
