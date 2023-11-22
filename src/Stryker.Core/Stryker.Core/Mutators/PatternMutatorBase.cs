using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

/// <summary> Base Mutator implementation for expressions with patterns </summary>
public abstract class PatternMutatorBase<T> : MutatorBase<T> where T : ExpressionSyntax
{
    public override MutationLevel MutationLevel => MutationLevel.Basic;

    /// <summary> Dictionary which maps original syntax kinds to target mutations </summary>
    /// <remarks>This could be a static field, but sonar does not like static fields for generic types, and the extra runtime cost is negligible.</remarks>
    private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate { get; } = new()
    {
        [SyntaxKind.OrPattern] = new[] { SyntaxKind.AndPattern },
        [SyntaxKind.AndPattern] = new[] { SyntaxKind.OrPattern },
        [SyntaxKind.LessThanEqualsToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken },
        [SyntaxKind.GreaterThanEqualsToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken },
        [SyntaxKind.LessThanToken] = new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken },
        [SyntaxKind.GreaterThanToken] = new[] { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken }
    };

    /// <summary> Apply mutations to a <see cref="PatternSyntax"/></summary>
    protected IEnumerable<Mutation> ApplyMutations(PatternSyntax node, SemanticModel semanticModel) => node switch
    {
        BinaryPatternSyntax binaryPattern => ApplyMutations(binaryPattern),
        RelationalPatternSyntax relationalPattern => ApplyMutations(relationalPattern),
        _ => Enumerable.Empty<Mutation>()
    };

    private IEnumerable<Mutation> ApplyMutations(BinaryPatternSyntax node)
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

    private IEnumerable<Mutation> ApplyMutations(RelationalPatternSyntax node)
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
