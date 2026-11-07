using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

/// <summary> Mutator implementation for is expression</summary>
public class IsPatternExpressionMutator : MutatorBase<IsPatternExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Basic;

    /// <summary>
    /// Apply mutations to all <see cref="PatternSyntax"/> inside an <see cref="IsPatternExpressionSyntax"/>.
    /// Apply mutations to the root pattern.
    /// </summary>
    public override IEnumerable<CsharpMutation> ApplyMutations(IsPatternExpressionSyntax node, SemanticModel semanticModel)
    {
        yield return ReverseRootPattern(node);
    }

    private static CsharpMutation ReverseRootPattern(IsPatternExpressionSyntax node) => node.Pattern switch
    {
        UnaryPatternSyntax notPattern => new CsharpMutation
        {
            OriginalNode = node,
            ReplacementNode = node.WithCleanTrivia().WithPattern(notPattern.WithCleanTrivia().Pattern),
            Type = Mutator.Equality,
            DisplayName = "Equality mutation"
        },
        _ => new CsharpMutation
        {
            OriginalNode = node,
            ReplacementNode = node.WithCleanTrivia().WithPattern(SyntaxFactory.UnaryPattern(node.Pattern.WithLeadingTrivia(SyntaxFactory.Space).WithoutTrailingTrivia())),
            Type = Mutator.Equality,
            DisplayName = "Equality mutation"
        }
    };
}
