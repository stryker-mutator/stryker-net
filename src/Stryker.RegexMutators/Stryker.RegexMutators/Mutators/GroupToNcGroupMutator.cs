using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.GroupNodes;

namespace Stryker.RegexMutators.Mutators;

public sealed class GroupToNcGroupMutator : RegexMutatorBase<CaptureGroupNode>, IRegexMutator
{
    public override IEnumerable<RegexMutation> ApplyMutations(CaptureGroupNode node, RegexNode root)
    {
        var replacementNode = new NonCaptureGroupNode(node.ChildNodes);

        yield return new RegexMutation
        {
            OriginalNode    = node,
            ReplacementNode = replacementNode,
            DisplayName     = "Regex capturing group to non-capturing group modification",
            Description =
                $"""Capturing group "{node}" was replaced with "{replacementNode}" at offset {node.GetSpan().Start}.""",
            ReplacementPattern = root.ReplaceNode(node, replacementNode).ToString()
        };
    }
}
