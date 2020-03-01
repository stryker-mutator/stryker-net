using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public static class MutantPlacer
    {

        private const string MutationConditional = "MutationConditional";
        private const string MutationIf = "MutationIf";
        private const string MutationHelper = "Helper";
        private const string HelperId = "-1";

        public static IEnumerable<string> MutationMarkers => new[] { MutationConditional, MutationIf, MutationHelper};

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block)
        {
            return SyntaxFactory.Block( 
                SyntaxFactory.UsingStatement(null, SyntaxFactory.ParseExpression(CodeInjection.StaticMarker), block));
        }

        public static IfStatementSyntax PlaceWithIfStatement(StatementSyntax original, StatementSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            return SyntaxFactory.IfStatement(
                condition: GetBinaryExpression(mutantId),
                statement: SyntaxFactory.Block(mutated),
                @else: SyntaxFactory.ElseClause(SyntaxFactory.Block(original)))
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIf, mutantId.ToString()));
        }

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            // remove the mutated node using its MutantPlacer remove method and update the tree
            if (nodeToRemove is IfStatementSyntax ifStatement)
            {
                return RemoveByIfStatement(ifStatement);
            }

            if (nodeToRemove is ParenthesizedExpressionSyntax parenthesizedExpression)
            {
                return RemoveByConditionalExpression(parenthesizedExpression);
            }

            if (nodeToRemove.GetAnnotatedNodes(MutationHelper).Any())
            {
                return SyntaxFactory.EmptyStatement();
            }
            // this is not one of our structure
            return nodeToRemove;
        }

        private static SyntaxNode RemoveByIfStatement(IfStatementSyntax ifStatement)
        {
            // return original statement
            var childNodes = ifStatement.Else.Statement.ChildNodes().ToList();
            return childNodes.Count == 1 ? childNodes[0] : ifStatement.Else.Statement;
        }

        public static ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original, ExpressionSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            return SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        condition: GetBinaryExpression(mutantId),
                        whenTrue: mutated,
                        whenFalse: original))
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationConditional, mutantId.ToString()));
        }

        public static T AnnotateHelper<T>(T node) where T:SyntaxNode
        {
            return node.WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper, HelperId));
        }

        private static SyntaxNode RemoveByConditionalExpression(ParenthesizedExpressionSyntax parenthesized)
        {
            if (parenthesized.Expression is ConditionalExpressionSyntax conditional)
            {
                // return original expression
                return conditional.WhenFalse;
            }

            return null;
        }

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutantId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private static ExpressionSyntax GetBinaryExpression(int mutantId)
        {
            return SyntaxFactory.ParseExpression(CodeInjection.SelectorExpression.Replace("ID", mutantId.ToString()));
        }
    }
}
