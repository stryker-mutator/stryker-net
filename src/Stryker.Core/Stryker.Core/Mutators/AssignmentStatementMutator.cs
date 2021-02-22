using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class AssignmentExpressionMutator : MutatorBase<AssignmentExpressionSyntax>, IMutator
    {
        private static readonly Dictionary<SyntaxKind, IEnumerable<SyntaxKind>> KindsToMutate = new Dictionary<SyntaxKind, IEnumerable<SyntaxKind>>
        {
            {SyntaxKind.AddAssignmentExpression, new [] { SyntaxKind.SubtractAssignmentExpression } },
            {SyntaxKind.SubtractAssignmentExpression, new [] { SyntaxKind.AddAssignmentExpression } },
            {SyntaxKind.MultiplyAssignmentExpression, new [] { SyntaxKind.DivideAssignmentExpression } },
            {SyntaxKind.DivideAssignmentExpression, new [] { SyntaxKind.MultiplyAssignmentExpression } },
            {SyntaxKind.ModuloAssignmentExpression, new [] { SyntaxKind.MultiplyAssignmentExpression } },
            {SyntaxKind.AndAssignmentExpression, new [] { SyntaxKind.OrAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression } },
            {SyntaxKind.OrAssignmentExpression, new [] { SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression} },
            {SyntaxKind.ExclusiveOrAssignmentExpression, new [] { SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression } },
            {SyntaxKind.LeftShiftAssignmentExpression, new [] { SyntaxKind.RightShiftAssignmentExpression } },
            {SyntaxKind.RightShiftAssignmentExpression, new [] { SyntaxKind.LeftShiftAssignmentExpression } },
        };

        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node)
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
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.AssignmentExpression(targetAssignmentKind, node.Left, node.Right),
                    DisplayName = $"{assignmentKind} to {targetAssignmentKind} mutation",
                    Type = Mutator.Assignment
                };
            }
        }
    }
}
