using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class AssignmentExpressionMutator : MutatorBase<AssignmentExpressionSyntax>
{
    private static readonly Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate = new()
    {
        { SyntaxKind.AddAssignmentExpression, new [] { SyntaxKind.SubtractAssignmentExpression } },
        { SyntaxKind.SubtractAssignmentExpression, new [] { SyntaxKind.AddAssignmentExpression } },
        { SyntaxKind.MultiplyAssignmentExpression, new [] { SyntaxKind.DivideAssignmentExpression } },
        { SyntaxKind.DivideAssignmentExpression, new [] { SyntaxKind.MultiplyAssignmentExpression } },
        { SyntaxKind.ModuloAssignmentExpression, new [] { SyntaxKind.MultiplyAssignmentExpression } },
        { SyntaxKind.AndAssignmentExpression, new [] { SyntaxKind.OrAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression } },
        { SyntaxKind.OrAssignmentExpression, new [] { SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression} },
        { SyntaxKind.ExclusiveOrAssignmentExpression, new [] { SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression } },
        { SyntaxKind.LeftShiftAssignmentExpression, new [] { SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.UnsignedRightShiftAssignmentExpression } },
        { SyntaxKind.RightShiftAssignmentExpression, new [] { SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.UnsignedRightShiftAssignmentExpression } },
        { SyntaxKind.CoalesceAssignmentExpression, new [] { SyntaxKind.SimpleAssignmentExpression } },
        { SyntaxKind.UnsignedRightShiftAssignmentExpression, new [] { SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression } },
    };

    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node, SemanticModel semanticModel)
    {
        var assignmentKind = node.Kind();

        if (assignmentKind == SyntaxKind.AddAssignmentExpression
            && (node.Left.IsAStringExpression() || node.Right.IsAStringExpression()))
        {
            yield break;
        }

        if (!KindsToMutate.TryGetValue(assignmentKind, out var targetAssignmentKinds))
        {
            yield break;
        }


        foreach (var targetAssignmentKind in targetAssignmentKinds)
        {
            var replacementNode =
                SyntaxFactory.AssignmentExpression(targetAssignmentKind, node.Left.WithCleanTrivia(), node.Right.WithCleanTrivia());
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithCleanTriviaFrom(node.OperatorToken));
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = $"{assignmentKind} to {targetAssignmentKind} mutation",
                Type = Mutator.Assignment
            };
        }
    }
}
