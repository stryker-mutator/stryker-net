using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    public class ConditionalInstrumentationEngine : IInstrumentCode
    {
        private readonly SyntaxAnnotation _marker;

        public ConditionalInstrumentationEngine(string marker)
        {
            _marker = new SyntaxAnnotation(marker, InstrumentEngineID);
        }

        public string InstrumentEngineID => "ConditionalInstrumentation";

        public  ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax condition, ExpressionSyntax original, ExpressionSyntax mutated) =>
            SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        condition: condition,
                        whenTrue: mutated,
                        whenFalse: original))
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(_marker);

        public SyntaxNode RemoveInstrumentation(SyntaxNode node)
        {
            if (node is ParenthesizedExpressionSyntax parenthesized && parenthesized.Expression is ConditionalExpressionSyntax conditional)
            {
                return conditional.WhenFalse;
            }
            throw new InvalidOperationException($"Expected a block containing a conditional expressionn, found:\n{node.ToFullString()}.");
        }
    }
}
