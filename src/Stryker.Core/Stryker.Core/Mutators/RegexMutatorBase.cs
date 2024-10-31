#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.RegexMutators;

namespace Stryker.Core.Mutators;

public abstract class RegexMutatorBase<T> : MutatorBase<T> where T : SyntaxNode
{
    protected abstract ILogger Logger { get; }

    protected abstract ExpressionSyntax? GetMutateableNode(T node, SemanticModel? semanticModel);

    public sealed override IEnumerable<Mutation> ApplyMutations(T node, SemanticModel? semanticModel)
    {
        var expression = GetMutateableNode(node, semanticModel);

        if (expression is null)
        {
            yield break;
        }

        foreach (var mutation in GenerateMutations(expression))
        {
            yield return mutation;
        }
    }

    private IEnumerable<Mutation> GenerateMutations(ExpressionSyntax patternExpression)
    {
        if (patternExpression?.Kind() != SyntaxKind.StringLiteralExpression)
        {
            yield break;
        }

        var currentValue = ((LiteralExpressionSyntax)patternExpression).Token.ValueText;
        var regexMutantOrchestrator = new RegexMutantOrchestrator(currentValue);
        var replacementValues = regexMutantOrchestrator.Mutate();

        foreach (var regexMutation in replacementValues)
        {
            try
            {
                _ = new Regex(regexMutation.ReplacementPattern);
            }
            catch (ArgumentException exception)
            {
                Logger?.LogDebug(
                                 "RegexMutator created mutation {CurrentValue} -> {ReplacementPattern} which is an invalid regular expression:\n{Message}",
                                 currentValue, regexMutation.ReplacementPattern, exception.Message);
                continue;
            }

            yield return new Mutation
            {
                OriginalNode = patternExpression,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                  SyntaxFactory.Literal(regexMutation
                                                                  .ReplacementPattern)),
                DisplayName = regexMutation.DisplayName,
                Type        = Mutator.Regex,
                Description = regexMutation.Description
            };
        }
    }
}
