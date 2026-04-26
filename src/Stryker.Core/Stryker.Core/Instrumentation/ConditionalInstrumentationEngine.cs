using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Injects a mutation controlled by a conditional operator.
/// </summary>
internal class ConditionalInstrumentationEngine : BaseEngine<ParenthesizedExpressionSyntax>
{
    /// <summary>
    /// Injects a conditional operator with the original code or the mutated one, depending on condition's result.
    /// </summary>
    /// <param name="condition">Expression for the condition.</param>
    /// <param name="original">Original code</param>
    /// <param name="mutated">Mutated code</param>
    /// <returns>A new expression containing the expected construct.</returns>
    public ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax condition, ExpressionSyntax original, ExpressionSyntax mutated) =>
        SyntaxFactory.ParenthesizedExpression(
                SyntaxFactory.ConditionalExpression(
                    condition: condition,
                    whenTrue: mutated,
                    whenFalse: original)).
        WithTriviaFrom(original).
        // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
        WithAdditionalAnnotations(Marker);

    protected override SyntaxNode Revert(ParenthesizedExpressionSyntax parenthesized) =>
        parenthesized.Expression is ConditionalExpressionSyntax conditional
            ? conditional.WhenFalse.WithTriviaFrom(parenthesized)
            : throw new InvalidOperationException(
                $"Expected a block containing a conditional expression, found:\n{parenthesized.ToFullString()}.");

    protected override bool ErasesAssignment(ParenthesizedExpressionSyntax node, string identifier) =>
        // check if identifier is assigned on false condition and not assigned on true
        node.Expression is ConditionalExpressionSyntax conditional
            ? conditional.WhenFalse.ContainsNodeThatVerifies(x =>
                  x.AssignsThis(identifier), false)
              && !conditional.WhenTrue.ContainsNodeThatVerifies(x =>
                  x.AssignsThis(identifier), false)
            : throw new InvalidOperationException($"Expected a conditional expression, found:\n{node.ToFullString()}.");
}
