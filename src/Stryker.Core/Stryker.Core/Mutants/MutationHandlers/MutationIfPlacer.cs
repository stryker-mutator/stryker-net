//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Collections.Generic;

//namespace Stryker.Core.Mutants.MutationHandlers
//{
//    /// <summary>
//    /// This is the base handler. If no other handler could handle the mutation it will be placed in an if statement.
//    /// </summary>
//    public class MutationIfPlacer
//    {
//        public SyntaxNode HandleInsertMutation(StatementSyntax original, StatementSyntax mutated, int mutantId)
//        {
//            // place the mutated statement inside the if statement
//            IfStatementSyntax mutantIf = SyntaxFactory.IfStatement(
//                condition: SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression,
//                SyntaxFactory.InvocationExpression(
//                    SyntaxFactory.MemberAccessExpression(
//                        SyntaxKind.SimpleMemberAccessExpression,
//                        SyntaxFactory.MemberAccessExpression(
//                            SyntaxKind.SimpleMemberAccessExpression,
//                            SyntaxFactory.IdentifierName("System"),
//                            SyntaxFactory.IdentifierName("Environment")
//                        ),
//                        SyntaxFactory.IdentifierName("GetEnvironmentVariable")),
//                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
//                        new List<ArgumentSyntax>() {
//                        SyntaxFactory.Argument(SyntaxFactory.ExpressionStatement(
//                            SyntaxFactory.LiteralExpression(
//                                SyntaxKind.StringLiteralExpression,
//                                SyntaxFactory.Literal("ActiveMutation"))).Expression)
//                        }
//                    ))
//                ),
//                SyntaxFactory.LiteralExpression(
//                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(mutantId.ToString()))),
//                statement: SyntaxFactory.Block(mutated),
//                @else: SyntaxFactory.ElseClause(SyntaxFactory.Block(original)))
//                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
//                .WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", mutantId.ToString()));

//            return mutantIf;
//        }

//        public SyntaxNode HandleRemoveMutation(SyntaxNode node)
//        {
//            if (node is IfStatementSyntax ifStatement)
//                return ifStatement.Else.Statement;
//            else return null;
//        }
//    }
//}
