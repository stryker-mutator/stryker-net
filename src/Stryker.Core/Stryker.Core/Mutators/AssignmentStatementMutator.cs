using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class AssignmentExpressionMutator : MutatorBase<AssignmentExpressionSyntax>, IMutator
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> KindsToMutate = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression},
            {SyntaxKind.SubtractAssignmentExpression, SyntaxKind.AddAssignmentExpression},
            {SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.DivideAssignmentExpression},
            {SyntaxKind.DivideAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression},
            {SyntaxKind.ModuloAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression},
            {SyntaxKind.AndAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression},
            {SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.AndAssignmentExpression},
            {SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression},
            {SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression},
        };

        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node)
        {
            var assignmentKind = node.Kind();
            if (KindsToMutate.TryGetValue(assignmentKind, out var targetAssignmentKind))
            {
                if (node.Kind() == SyntaxKind.AddAssignmentExpression 
                    && (node.Left.IsAStringExpression() || node.Right.IsAStringExpression()))
                {
                    yield break;
                }
                
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
