using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;

namespace Stryker.RegexMutators.Mutators;

public sealed class QuantifierShortMutator : RegexMutatorBase<QuantifierNode>, IRegexMutator
{
    private static readonly QuantifierQuantityMutator          _innerNm      = new();
    private static readonly QuantifierUnlimitedQuantityMutator _innerNOrMore = new();

    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(QuantifierNode node, RegexNode root) =>
        node switch
        {
            QuantifierNMNode or QuantifierNNode or QuantifierNOrMoreNode => [],
            QuantifierStarNode => PlusOrStar(node, root, 0),
            QuantifierPlusNode => PlusOrStar(node, root, 1),
            QuantifierQuestionMarkNode => QuestionMark(node, root),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };

    private static IEnumerable<RegexMutation> QuestionMark(QuantifierNode node, RegexNode root)
    {
        var newNode = node.Parent.ReplaceNode(node, new QuantifierNMNode(0, 1, node.ChildNodes.First()), false)
                       .ChildNodes.OfType<QuantifierNMNode>().First();

        return _innerNm.ApplyMutations(newNode, root)
                    .Select(a => new RegexMutation
                        {
                            OriginalNode    = node,
                            ReplacementNode = a.ReplacementNode,
                            DisplayName     = a.DisplayName,
                            Description =
                                a.Description.Replace($"\"{a.OriginalNode}\"", $"\"{node}\""),
                            ReplacementPattern = root.ReplaceNode(node, a.ReplacementNode).ToString()
                        });
    }

    private static IEnumerable<RegexMutation> PlusOrStar(QuantifierNode node, RegexNode root, int n)
    {
        var newNode = node.Parent.ReplaceNode(node, new QuantifierNOrMoreNode(n, node.ChildNodes.First()), false)
                       .ChildNodes.OfType<QuantifierNOrMoreNode>().First();

        return _innerNOrMore.ApplyMutations(newNode, root)
                         .Select(a => new RegexMutation
                             {
                                 OriginalNode    = node,
                                 ReplacementNode = a.ReplacementNode,
                                 DisplayName     = a.DisplayName,
                                 Description =
                                     a.Description.Replace($"\"{a.OriginalNode}\"", $"\"{node}\""),
                                 ReplacementPattern =
                                     root.ReplaceNode(node, a.ReplacementNode).ToString()
                             });
    }
}
