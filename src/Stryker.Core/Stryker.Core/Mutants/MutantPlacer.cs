using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public static class MutantPlacer
    {
        private const string Mutationconditional = "MutationConditional";
        private const string Mutationif = "MutationIf";
        private static readonly string helper;

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
        }

        public static string[] MutationMakers => new[] {Mutationconditional, Mutationif};

        public static SyntaxTree ActiveMutantSelectorHelper => CSharpSyntaxTree.ParseText(helper);

        public static IfStatementSyntax PlaceWithIfStatement(StatementSyntax original, StatementSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            return SyntaxFactory.IfStatement(
                condition: GetBinaryExpression(mutantId),
                statement: SyntaxFactory.Block(mutated),
                @else: SyntaxFactory.ElseClause(SyntaxFactory.Block(original)))
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(Mutationif, mutantId.ToString()));
        }

        public static SyntaxNode RemoveByIfStatement(SyntaxNode node)
        {
            if (!(node is IfStatementSyntax ifStatement))
            {
                return null;
            }
            // return original statement
            var sub = ifStatement.Else.Statement.ChildNodes().ToList();
            return sub.Count == 1 ? sub[0] : ifStatement.Else.Statement;

        }

        public static ConditionalExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original, ExpressionSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            return SyntaxFactory.ConditionalExpression(
                condition: GetBinaryExpression(mutantId),
                whenTrue: mutated,
                whenFalse: original)
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(Mutationconditional, mutantId.ToString()));
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
            return SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression,
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Stryker"),
                            SyntaxFactory.IdentifierName("ActiveMutationHelper")
                        ),
                        SyntaxFactory.IdentifierName("ActiveMutation")
                ),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(mutantId)));
        }
    }
}
