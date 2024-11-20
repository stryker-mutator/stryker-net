using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutators;

public class BinaryPatternMutator : MutatorBase<BinaryPatternSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Basic;

    private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate { get; } = new()
    {
        [SyntaxKind.OrPattern] = [SyntaxKind.AndPattern],
        [SyntaxKind.AndPattern] = [SyntaxKind.OrPattern],
    };

    public override IEnumerable<Mutation> ApplyMutations(BinaryPatternSyntax node, SemanticModel semanticModel)
    {
        if (!KindsToMutate.TryGetValue(node.Kind(), out var mutations))
        {
            yield break;
        }

        foreach (var mutation in mutations)
        {
            // can't use the update method here, because roslyn implementation is broken
            var replacementNode = SyntaxFactory.BinaryPattern(mutation, node.Left, node.Right);
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));
            yield return new()
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Logical mutation",
                Type = Mutator.Logical
            };
        }

    }
}
