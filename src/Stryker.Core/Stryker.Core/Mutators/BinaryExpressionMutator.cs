using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class BinaryExpressionMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
    {
        private Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> _kindsToMutate { get; set; }

        public BinaryExpressionMutator()
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
                // skip string additions
                if (node.Kind() == SyntaxKind.AddExpression &&(node.Left.IsAStringExpression()||node.Right.IsAStringExpression()))
                {
                    yield break;
                }
                
                foreach(var mutationKind in _kindsToMutate[node.Kind()])
                {
                    var replacementNode = SyntaxFactory.BinaryExpression(mutationKind, node.Left, node.Right);
                    // make sure the trivia stays in place for displaying
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

                    yield return new Mutation()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = "Binary expression mutation",
                        Type = GetMutatorType(mutationKind)
                    };
                }
            }
        }

        private Mutator GetMutatorType(SyntaxKind kind)
        {
            string kindString = kind.ToString();
            if (kindString.StartsWith("Logical"))
            {
                return Mutator.Logical;
            } else if (kindString.Contains("Equals") 
                || kindString.Contains("Greater") 
                || kindString.Contains("Less"))
            {
                return Mutator.Equality;
            } else
            {
                return Mutator.Arithmetic;
            }
        }
    }
}
