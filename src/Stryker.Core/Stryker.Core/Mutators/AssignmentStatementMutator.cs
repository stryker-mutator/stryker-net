using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class AssignmentStatementMutator : Mutator<AssignmentExpressionSyntax>, IMutator
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

        public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node)
        {
            var assignmentKind = node.Kind();
            if (KindsToMutate.TryGetValue(assignmentKind, out var targetAssignementKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.AssignmentExpression(targetAssignementKind, node.Left, node.Right),
                    DisplayName = $"{assignmentKind} to {targetAssignementKind} mutation",
                    Type = MutatorType.Assignment
                };
            }
        }
    }
}
