using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutators;

public class RelationalPatternMutator : MutatorBase<RelationalPatternSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Basic;

    private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate { get; } = new()
    {
        [SyntaxKind.LessThanEqualsToken] = [SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken],
        [SyntaxKind.GreaterThanEqualsToken] = [SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken],
        [SyntaxKind.LessThanToken] = [SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken],
        [SyntaxKind.GreaterThanToken] = [SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken]
    };

    public override IEnumerable<Mutation> ApplyMutations(RelationalPatternSyntax node, SemanticModel semanticModel)
    {
        if (!KindsToMutate.TryGetValue(node.OperatorToken.Kind(), out var mutations))
        {
            yield break;
        }

        foreach (var mutation in mutations)
        {
            yield return new()
            {
                OriginalNode = node,
                ReplacementNode = node.WithOperatorToken(SyntaxFactory.Token(mutation).WithTriviaFrom(node.OperatorToken)),
                DisplayName = "Equality mutation",
                Type = Mutator.Equality
            };
        }
    }
}
