using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class BinaryExpressionMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
    {
        private readonly Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> _kindsToMutate;

        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public BinaryExpressionMutator()
        {
            _kindsToMutate = new Dictionary<SyntaxKind, IEnumerable<SyntaxKind>>
            {
                {SyntaxKind.SubtractExpression, new List<SyntaxKind> { SyntaxKind.AddExpression} },
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
                {SyntaxKind.LeftShiftExpression, new List<SyntaxKind> {SyntaxKind.RightShiftExpression } },
                {SyntaxKind.RightShiftExpression, new List<SyntaxKind> {SyntaxKind.LeftShiftExpression } },
                {SyntaxKind.BitwiseOrExpression, new List<SyntaxKind> {SyntaxKind.BitwiseAndExpression } },
                {SyntaxKind.BitwiseAndExpression, new List<SyntaxKind> {SyntaxKind.BitwiseOrExpression } },
            };
        }

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            // skip string additions
            if (node.Kind() == SyntaxKind.AddExpression && (node.Left.IsAStringExpression()|| node.Right.IsAStringExpression()))
            {
                yield break;
            }

            if(_kindsToMutate.ContainsKey(node.Kind()))
            {
                foreach(var mutationKind in _kindsToMutate[node.Kind()])
                {
                    var replacementNode = SyntaxFactory.BinaryExpression(mutationKind, node.Left, node.Right);
                    // make sure the trivia stays in place for displaying
                    replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));
                    var mutatorType = GetMutatorType(mutationKind);
                    yield return new Mutation()
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = $"{mutatorType} mutation",
                        Type = mutatorType
                    };
                }
            }
            else if (node.Kind() == SyntaxKind.ExclusiveOrExpression)
            {
                // Place both a logical and an integral mutation. Only one will compile, the other will be removed. See: https://github.com/stryker-mutator/stryker-net/issues/664
                yield return GetLogicalMutation(node);
                yield return GetIntegralMutation(node);
            }
        }

        private static Mutator GetMutatorType(SyntaxKind kind)
        {
            var kindString = kind.ToString();
            if (kindString.StartsWith("Logical"))
            {
                return Mutator.Logical;
            }

            if (kindString.Contains("Equals") 
                || kindString.Contains("Greater") 
                || kindString.Contains("Less"))
            {
                return Mutator.Equality;
            }
            if (kindString.StartsWith("Bitwise") || kindString.Contains("Shift"))
            {
                return Mutator.Bitwise;
            }
            return Mutator.Arithmetic;
        }

        private static Mutation GetLogicalMutation(BinaryExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, node.Left, node.Right);
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Logical mutation",
                Type = Mutator.Logical
            };
        }

        private static Mutation GetIntegralMutation(ExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, SyntaxFactory.ParenthesizedExpression(node));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Bitwise mutation",
                Type = Mutator.Bitwise
            };
        }
    }
}
