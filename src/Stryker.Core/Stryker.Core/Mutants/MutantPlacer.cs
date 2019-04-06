﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Stryker.Core.Compiling;

namespace Stryker.Core.Mutants
{
    public static class MutantPlacer
    {
        public static readonly string HelperNamespace = GetRandomNamespace();

        private const string MutationConditional = "MutationConditional";
        private const string MutationIf = "MutationIf";
            using (var stream = typeof(MutantPlacer).Assembly.GetManifestResourceStream("Stryker.Core.Mutants.ActiveMutationHelper.cs"))
            using (var reader = new StreamReader(stream))
            {
                helper = reader.ReadToEnd().Replace("StrykerNamespace", HelperNamespace);


        public static IEnumerable<string> MutationMarkers => new[] {MutationConditional, MutationIf};

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
                return MutantPlacer.RemoveByIfStatement(ifStatement);
            } else if (nodeToRemove is ParenthesizedExpressionSyntax parenthesizedExpression)
            {
                return MutantPlacer.RemoveByConditionalExpression(parenthesizedExpression);
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

        private static string GetRandomNamespace()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] chars = new char[15];
            for (int i = 0; i < 15; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return "Stryker" + new string(chars);
        }
    }
}
