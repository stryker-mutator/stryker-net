using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    internal class ConditionalAccessExpressionMutator : MutatorBase<ConditionalAccessExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ConditionalAccessExpressionSyntax node)
        {
            var whenNotNull = node.WhenNotNull;
            switch (whenNotNull)
            {
                case ConditionalAccessExpressionSyntax:
                    yield return CreateConditionalAccessExpressionMutation(node);
                    yield break;
                case MemberBindingExpressionSyntax:
                    yield return CreateMemberBindingExpressionMutation(node);
                    yield break;
                case InvocationExpressionSyntax:
                    yield return CreateInvocationExpressionMutation(node);
                    yield break;
                case ElementBindingExpressionSyntax:
                    yield break; // TODO: Implement
                default:
                    yield break;
            }
        }

        private static Mutation CreateConditionalAccessExpressionMutation(ConditionalAccessExpressionSyntax node)
        {
            var whenNotNullExpression = (node.WhenNotNull as ConditionalAccessExpressionSyntax).Expression;
            var conditionalAccesExpression = node.WhenNotNull as ConditionalAccessExpressionSyntax;

            return whenNotNullExpression switch
            {
                MemberBindingExpressionSyntax memberBindingExpression => CreateConditionalAccessMemberBindingExpressionMutation(node, conditionalAccesExpression, memberBindingExpression),
                InvocationExpressionSyntax invocationExpression => CreateConditionalAccessInvocationExpressionMutation(node, conditionalAccesExpression, invocationExpression),
                ElementBindingExpressionSyntax elementBindingExpression => null,//TODO: implement
                _ => null,
            };
        }

        private static Mutation CreateConditionalAccessInvocationExpressionMutation(ConditionalAccessExpressionSyntax node, ConditionalAccessExpressionSyntax conditionalAccesExpression, InvocationExpressionSyntax invocationExpression)
        {
            var memberBindingExpression = invocationExpression.Expression as MemberBindingExpressionSyntax;
            var expression = CreateMemberAccessExpression(node, memberBindingExpression);

            var left = SyntaxFactory.InvocationExpression(expression, invocationExpression.ArgumentList);
            var right = conditionalAccesExpression.WhenNotNull;
            var replacementNode = SyntaxFactory.ConditionalAccessExpression(left, right);

            return new Mutation()
            {
                OriginalNode = node,
                DisplayName = "Conditional access expression",
                ReplacementNode = replacementNode,
                Type = Mutator.Access
            };
        }

        private static Mutation CreateConditionalAccessMemberBindingExpressionMutation(ConditionalAccessExpressionSyntax node, ConditionalAccessExpressionSyntax conditionalAccesExpression, MemberBindingExpressionSyntax memberBindingExpression)
        {
            var leftHandSide = CreateMemberAccessExpression(node, memberBindingExpression);

            var rightHandSide = conditionalAccesExpression.WhenNotNull;

            var replacementNode = SyntaxFactory.ConditionalAccessExpression(
                leftHandSide,
                rightHandSide);

            return new Mutation()
            {
                OriginalNode = node,
                DisplayName = "Conditional access expression",
                ReplacementNode = replacementNode,
                Type = Mutator.Access
            };
        }

        private static Mutation CreateMemberBindingExpressionMutation(ConditionalAccessExpressionSyntax node)
        {
            var memberBindingExpression = node.WhenNotNull as MemberBindingExpressionSyntax;

            var replacementNode = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                node.Expression,
                SyntaxFactory.Token(SyntaxKind.DotToken),
                memberBindingExpression.Name);

            return new Mutation()
            {
                OriginalNode = node,
                DisplayName = "Conditional access expression",
                ReplacementNode = replacementNode,
                Type = Mutator.Access
            };
        }

        private static Mutation CreateInvocationExpressionMutation(ConditionalAccessExpressionSyntax node)
        {
            var invocationExpression = node.WhenNotNull as InvocationExpressionSyntax;

            var leftExpr = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    node.Expression,
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    (invocationExpression.Expression as MemberBindingExpressionSyntax).Name);

            var replacementNode = SyntaxFactory.InvocationExpression(leftExpr, invocationExpression.ArgumentList);

            return new Mutation()
            {
                OriginalNode = node,
                DisplayName = "Conditional access expression",
                ReplacementNode = replacementNode,
                Type = Mutator.Access
            };
        }

        private static MemberAccessExpressionSyntax CreateMemberAccessExpression(ConditionalAccessExpressionSyntax node, MemberBindingExpressionSyntax memberBindingExpression)
        => SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            node.Expression,
            SyntaxFactory.Token(SyntaxKind.DotToken),
            memberBindingExpression.Name);
    }
}
