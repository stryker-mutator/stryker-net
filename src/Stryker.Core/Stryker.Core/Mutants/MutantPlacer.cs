using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutants
{
    public static class MutantPlacer
    {
        private const string MutationConditional = "MutationConditional";
        private const string MutationIf = "MutationIf";

        private const string PatternForCheck = "\\/\\/ *check with: *([^\\r\\n]+)";


        private static readonly string SelectorExpression;
        private static readonly IList<SyntaxTree> Helpers = new List<SyntaxTree>();

        static MutantPlacer()
        {
            var helper = GetSourceFromResource("Stryker.Core.Mutants.ActiveMutationHelper.cs");
            var extractor = new Regex(PatternForCheck);
            var result = extractor.Match(helper);
            if (!result.Success)
            {
                throw new InvalidDataException("Internal error: failed to find expression for mutant selection.");
            }
            SelectorExpression = result.Groups[1].Value;

            Helpers.Add(CSharpSyntaxTree.ParseText(helper, new CSharpParseOptions(LanguageVersion.Latest)));
            helper = GetSourceFromResource("Stryker.Core.Coverage.CoverageChannel.cs");
            Helpers.Add(CSharpSyntaxTree.ParseText(helper, new CSharpParseOptions(LanguageVersion.Latest)));
        }

        private static string GetSourceFromResource(string sourceResourceName)
        {
            string helper;
            using (var stream =
                typeof(MutantPlacer).Assembly.GetManifestResourceStream(sourceResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    helper = reader.ReadToEnd();
                }
            }

            return helper;
        }

        public static IEnumerable<string> MutationMarkers => new[] {MutationConditional, MutationIf};

        public static IEnumerable<SyntaxTree> ActiveMutantSelectorHelper => Helpers;

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

        public static SyntaxNode RemoveByIfStatement(SyntaxNode node)
        {
            if (!(node is IfStatementSyntax ifStatement))
            {
                return null;
            }
            // return original statement
            var childNodes = ifStatement.Else.Statement.ChildNodes().ToList();
            return childNodes.Count == 1 ? childNodes[0] : ifStatement.Else.Statement;
        }

        public static ConditionalExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original, ExpressionSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            return SyntaxFactory.ConditionalExpression(
                condition: GetBinaryExpression(mutantId),
                whenTrue: mutated,
                whenFalse: original)
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationConditional, mutantId.ToString()));
        }

        public static SyntaxNode RemoveByConditionalExpression(SyntaxNode node)
        {
            if (node is ConditionalExpressionSyntax conditional)
            {
                // return original expression
                return conditional.WhenFalse;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutantId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private static ExpressionSyntax GetBinaryExpression(int mutantId)
        {
            return SyntaxFactory.ParseExpression(SelectorExpression.Replace("ID", mutantId.ToString()));
        }
    }
}
