using System.Collections.Generic;
using System.Linq;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;

namespace Stryker.RegexMutators.Mutators;

public sealed class CharacterClassChildRemovalMutator : RegexMutatorBase<CharacterClassNode>, IRegexMutator
{
    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassNode node, RegexNode root)
    {
        if (node.Subtraction is not null)
        {
            yield return new RegexMutation
            {
                OriginalNode    = node.Subtraction,
                ReplacementNode = null,
                DisplayName     = "Regex character class subtraction removal",
                Description =
                    $"""Character Class Subtraction "-{node.Subtraction}" was removed at offset {node.Subtraction.GetSpan().Start - 1}.""",
                ReplacementPattern = root.RemoveNode(node.Subtraction).ToString()
            };

            if (node.CharacterSet.ChildNodes.Count() == 1)
            {
                var replacementNode2 = new CharacterClassNode(node.Subtraction.CharacterSet, node.Negated);

                yield return new RegexMutation
                {
                    OriginalNode    = node,
                    ReplacementNode = replacementNode2,
                    DisplayName     = "Regex character class subtraction replacement",
                    Description =
                        $"""Character Class "{node}" was replace with its subtraction "{replacementNode2}" at offset {node.GetSpan().Start}.""",
                    ReplacementPattern = root.ReplaceNode(node, replacementNode2).ToString()
                };
            }
        }

        if (node.CharacterSet.ChildNodes.Count() == 1)
        {
            yield break;
        }

        foreach (var childNode in node.CharacterSet.ChildNodes)
        {
            yield return new RegexMutation
            {
                OriginalNode    = childNode,
                ReplacementNode = null,
                DisplayName     = "Regex character class child removal",
                Description =
                    $"""Removed child "{childNode}" from character class "{node}" at offset {node.GetSpan().Start}.""",
                ReplacementPattern = root.RemoveNode(childNode).ToString()
            };
        }
    }
}
