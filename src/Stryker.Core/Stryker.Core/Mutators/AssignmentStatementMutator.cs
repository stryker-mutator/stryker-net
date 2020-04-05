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
            {SyntaxKind.AddAssignmentExpression, new List<SyntaxKind> { SyntaxKind.SubtractAssignmentExpression } },
            {SyntaxKind.SubtractAssignmentExpression, new List<SyntaxKind> { SyntaxKind.AddAssignmentExpression } },
            {SyntaxKind.MultiplyAssignmentExpression, new List<SyntaxKind> { SyntaxKind.DivideAssignmentExpression } },
            {SyntaxKind.DivideAssignmentExpression, new List<SyntaxKind> { SyntaxKind.MultiplyAssignmentExpression } },
            {SyntaxKind.ModuloAssignmentExpression, new List<SyntaxKind> { SyntaxKind.MultiplyAssignmentExpression } },
            {SyntaxKind.AndAssignmentExpression, new List<SyntaxKind> { SyntaxKind.OrAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression } },
            {SyntaxKind.OrAssignmentExpression, new List<SyntaxKind> { SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression } },
            {SyntaxKind.ExclusiveOrAssignmentExpression, new List<SyntaxKind> { SyntaxKind.OrAssignmentExpression, SyntaxKind.AndAssignmentExpression } },
            {SyntaxKind.LeftShiftAssignmentExpression, new List<SyntaxKind> { SyntaxKind.RightShiftAssignmentExpression } },
            {SyntaxKind.RightShiftAssignmentExpression, new List<SyntaxKind> { SyntaxKind.LeftShiftAssignmentExpression } },
        };

        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node)
        {
            var assignmentKind = node.Kind();
            if (KindsToMutate.TryGetValue(assignmentKind, out var targetAssignmentKinds))
            {
                if (node.Kind() == SyntaxKind.AddAssignmentExpression 
                    && (node.Left.IsAStringExpression() || node.Right.IsAStringExpression()))
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
}
