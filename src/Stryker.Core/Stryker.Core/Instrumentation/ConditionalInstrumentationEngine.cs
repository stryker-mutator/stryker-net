using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects a mutation controlled by a conditional operator.
    /// </summary>
    internal class ConditionalInstrumentationEngine : BaseEngine<ParenthesizedExpressionSyntax>
    {

        public ConditionalInstrumentationEngine(string marker): base(marker)
        {
        }

        /// <summary>
        /// Injects a conditional operator with the original code or the mutated one, depending on condition's result.
        /// </summary>
        /// <param name="condition">Expression for the condition.</param>
        /// <param name="original">Original code</param>
        /// <param name="mutated">Mutated code</param>
        /// <returns>A new expression containing the expected construct.</returns>
        public  ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax condition, ExpressionSyntax original, ExpressionSyntax mutated) =>
            SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        condition: condition,
                        whenTrue: mutated,
                        whenFalse: original))
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(Marker);

        protected override SyntaxNode Revert(ParenthesizedExpressionSyntax parenthesized)
        {
            if (parenthesized.Expression is ConditionalExpressionSyntax conditional)
            {
                return conditional.WhenFalse;
            }
            throw new InvalidOperationException($"Expected a block containing a conditional expression, found:\n{parenthesized.ToFullString()}.");
        }
    }
}
