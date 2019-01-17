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

        private const string patternForCheck = "\\/\\/ *check with: *([^\\r\\n]+)";
        private static readonly string helper;
        private static readonly string selectorExpression;

        static MutantPlacer()
        {
            using (var stream =
                typeof(MutantPlacer).Assembly.GetManifestResourceStream("Stryker.Core.Mutants.ActiveMutationHelper.cs"))
            {
                using (var reader = new StreamReader(stream))
                {
                    helper = reader.ReadToEnd();
                }
            }

            var extractor = new Regex(patternForCheck);
            var result = extractor.Match(helper);
            if (!result.Success)
            {
                throw new InvalidDataException("Internal error: failed to find expression for mutant selection.");
            }

            selectorExpression = result.Groups[1].Value;
        }

        public static string[] MutationMarkers => new[] {MutationConditional, MutationIf};

        public static SyntaxTree ActiveMutantSelectorHelper => CSharpSyntaxTree.ParseText(helper, options: new CSharpParseOptions(LanguageVersion.Latest));

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
            return SyntaxFactory.ParseExpression(selectorExpression.Replace("ID", mutantId.ToString()));
        }
    }
}
