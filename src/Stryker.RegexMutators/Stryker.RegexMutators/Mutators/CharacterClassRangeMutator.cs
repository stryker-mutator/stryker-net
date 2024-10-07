using System;
using System.Collections.Generic;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;

namespace Stryker.RegexMutators.Mutators;

public sealed class CharacterClassRangeMutator : RegexMutatorBase<CharacterClassRangeNode>, IRegexMutator
{
    /// <inheritdoc />
    public override IEnumerable<RegexMutation> ApplyMutations(CharacterClassRangeNode node, RegexNode root)
    {
        foreach (var regexMutation in MutateRangePart(root, node.Start, node.End, ValidateStartNode))
        {
            yield return regexMutation;
        }

        foreach (var regexMutation in MutateRangePart(root, node.End, node.Start, ValidateEndNode))
        {
            yield return regexMutation;
        }
    }

    private static RegexNode MutateNode(RegexNode n, int offset) =>
        n switch
        {
            CharacterNode cn => new CharacterNode((char)(cn.Character + offset)),
            EscapeCharacterNode ecn when ecn.Escape[0] == 'x' =>
                EscapeCharacterNode.FromHex(Convert.ToString(ecn.Character + offset, 16).PadLeft(2, '0')),
            EscapeCharacterNode ecn when ecn.Escape[0] == 'u' =>
                EscapeCharacterNode.FromUnicode(Convert.ToString(ecn.Character + offset, 16).PadLeft(4, '0')),
            EscapeCharacterNode ecn when char.IsDigit(ecn.Escape[0]) =>
                EscapeCharacterNode.FromOctal(Convert.ToString(ecn.Character + offset, 8).PadLeft(3, '0')),
            _ => null
        };

    private bool ValidateStartNode(RegexNode mutatedStartNode, RegexNode originalEndNode) =>
        (mutatedStartNode, originalEndNode) switch
        {
            (CharacterNode cs, CharacterNode ce) => cs.Character <= ce.Character && char.IsLetterOrDigit(cs.Character),
            (CharacterNode cs, EscapeCharacterNode ece) => cs.Character <= ece.Character &&
                                                           char.IsLetterOrDigit(cs.Character),
            (EscapeCharacterNode ecs, CharacterNode ce) => ecs.Character        <= ce.Character,
            (EscapeCharacterNode ecs, EscapeCharacterNode ece) => ecs.Character <= ece.Character,
            _ => false
        };

    private bool ValidateEndNode(RegexNode mutatedEndNode, RegexNode originalStartNode) =>
        (originalStartNode, mutatedEndNode) switch
        {
            (CharacterNode cs, CharacterNode ce) => cs.Character <= ce.Character && char.IsLetterOrDigit(ce.Character),
            (CharacterNode cs, EscapeCharacterNode ece) => cs.Character <= ece.Character,
            (EscapeCharacterNode ecs, CharacterNode ce) => ecs.Character <= ce.Character &&
                                                           char.IsLetterOrDigit(ce.Character),
            (EscapeCharacterNode ecs, EscapeCharacterNode ece) => ecs.Character <= ece.Character,
            _ => false
        };

    private static IEnumerable<RegexMutation> MutateRangePart(RegexNode                        root,
                                                              RegexNode                        originalNode,
                                                              RegexNode                        otherNode,
                                                              Func<RegexNode, RegexNode, bool> validator)
    {
        var (start, _) = originalNode.GetSpan();

        foreach (var replacementNode in new[] { MutateNode(originalNode, -1), MutateNode(originalNode, +1) })
        {
            if (!validator(replacementNode, otherNode) || originalNode == replacementNode)
            {
                continue;
            }

            yield return new RegexMutation
            {
                OriginalNode    = originalNode,
                ReplacementNode = replacementNode,
                DisplayName     = "Regex character class range modification",
                Description =
                    $"""Replaced character "{originalNode}" with "{replacementNode}" at offset {start}.""",
                ReplacementPattern = root.ReplaceNode(originalNode, replacementNode).ToString()
            };
        }
    }
}
