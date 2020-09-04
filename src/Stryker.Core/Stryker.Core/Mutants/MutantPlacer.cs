using System;
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

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
            SyntaxFactory.Block( 
                SyntaxFactory.UsingStatement(null, SyntaxFactory.ParseExpression(CodeInjection.StaticMarker), block));


        public static BlockSyntax AsBlock(StatementSyntax input) =>  (input is BlockSyntax block) ? block : SyntaxFactory.Block(input);

        public static IfStatementSyntax PlaceWithIfStatement(StatementSyntax original, StatementSyntax mutated, int mutantId) =>
            SyntaxFactory.IfStatement(
                    condition: GetBinaryExpression(mutantId),
                    statement: AsBlock(mutated),
                    @else: SyntaxFactory.ElseClause( AsBlock(original)))
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIf, mutantId.ToString()));

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove) =>
            nodeToRemove switch
            {
                // remove the mutated node using its MutantPlacer remove method and update the tree
                IfStatementSyntax ifStatement => RemoveByIfStatement(ifStatement),
                ParenthesizedExpressionSyntax parenthesizedExpression => RemoveByConditionalExpression(
                    parenthesizedExpression),
                _ => nodeToRemove.GetAnnotatedNodes(MutationHelper).Any()
                    ? SyntaxFactory.EmptyStatement()
                    : nodeToRemove
            };

        private static SyntaxNode RemoveByIfStatement(IfStatementSyntax ifStatement)
        {
            // return original statement
            var childNodes = ifStatement.Else.Statement.ChildNodes().ToList();
            return childNodes.Count == 1 ? childNodes[0] : ifStatement.Else.Statement;
        }

        public static ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original, ExpressionSyntax mutated, int mutantId) =>
            SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        condition: GetBinaryExpression(mutantId),
                        whenTrue: mutated,
                        whenFalse: original))
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationConditional, mutantId.ToString()));

        // us this method to annotate injected helper code. I.e. any injected code that is NOT a mutation but provides some infrastructure for the mutant to run
        public static T AnnotateHelper<T>(T node) where T:SyntaxNode => node.WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper, HelperId));

        private static SyntaxNode RemoveByConditionalExpression(ParenthesizedExpressionSyntax parenthesized) => 
            parenthesized.Expression is ConditionalExpressionSyntax conditional ? conditional.WhenFalse : null;

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutantId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private static ExpressionSyntax GetBinaryExpression(int mutantId) => 
            SyntaxFactory.ParseExpression(CodeInjection.SelectorExpression.Replace("ID", mutantId.ToString()));
    }
}
