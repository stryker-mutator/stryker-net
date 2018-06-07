using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class BinairyExpressionMutator : Mutator<BinaryExpressionSyntax>, IMutator
    {
        private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> _kindsToMutate { get; set; }

        public BinairyExpressionMutator()
        {
            _kindsToMutate = new Dictionary<SyntaxKind, IEnumerable<SyntaxKind>>
            {
                {SyntaxKind.SubtractExpression, new List<SyntaxKind> { SyntaxKind.AddExpression } },
                {SyntaxKind.AddExpression, new List<SyntaxKind> {SyntaxKind.SubtractExpression } },
                {SyntaxKind.MultiplyExpression, new List<SyntaxKind> {SyntaxKind.DivideExpression } },
                {SyntaxKind.DivideExpression, new List<SyntaxKind> {SyntaxKind.MultiplyExpression } },
                {SyntaxKind.ModuloExpression, new List<SyntaxKind> {SyntaxKind.MultiplyExpression } },
                {SyntaxKind.GreaterThanExpression, new List<SyntaxKind> {SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanOrEqualExpression } },
                {SyntaxKind.LessThanExpression, new List<SyntaxKind> {SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanOrEqualExpression } },
                {SyntaxKind.GreaterThanOrEqualExpression, new List<SyntaxKind> { SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression } },
                {SyntaxKind.LessThanOrEqualExpression, new List<SyntaxKind> { SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression } },
                {SyntaxKind.EqualsExpression, new List<SyntaxKind> {SyntaxKind.NotEqualsExpression } },
                {SyntaxKind.NotEqualsExpression, new List<SyntaxKind> {SyntaxKind.EqualsExpression } },
                {SyntaxKind.LogicalAndExpression, new List<SyntaxKind> {SyntaxKind.LogicalOrExpression } },
                {SyntaxKind.LogicalOrExpression, new List<SyntaxKind> {SyntaxKind.LogicalAndExpression } },
            };
        }

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            if(_kindsToMutate.ContainsKey(node.Kind()))
            {
                foreach(var mutationKind in _kindsToMutate[node.Kind()])
                {
                    var replacementNode = SyntaxFactory.BinaryExpression(mutationKind, node.Left, node.Right);
                    // make sure the trivia stays in place for displaying
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

                    yield return new Mutation()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = "Binairy expression mutation",
                        Type = "MathMutator"
                    };
                }
            }
        }
    }
}
