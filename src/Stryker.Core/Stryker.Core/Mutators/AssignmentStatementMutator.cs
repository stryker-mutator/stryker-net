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
                    Type = nameof(AssignmentStatementMutator)
                };
            }
        }
    }
}
/*

        case SyntaxKind.LeftShiftAssignmentExpression:
          return SyntaxKind.LessThanLessThanEqualsToken;
        case SyntaxKind.RightShiftAssignmentExpression:
          return SyntaxKind.GreaterThanGreaterThanEqualsToken;
 
 
 
        case SyntaxKind.AndAssignmentExpression:
          return SyntaxKind.AmpersandEqualsToken;
        case SyntaxKind.ExclusiveOrAssignmentExpression:
          return SyntaxKind.CaretEqualsToken;
        case SyntaxKind.OrAssignmentExpression:
          return SyntaxKind.BarEqualsToken;
        case SyntaxKind.ModuloAssignmentExpression:
          return SyntaxKind.PercentEqualsToken;         
                  case SyntaxKind.MultiplyAssignmentExpression:
          return SyntaxKind.AsteriskEqualsToken;
        case SyntaxKind.DivideAssignmentExpression:
          return SyntaxKind.SlashEqualsToken;
          
        case SyntaxKind.SimpleAssignmentExpression:
          return SyntaxKind.EqualsToken;
        case SyntaxKind.AddAssignmentExpression:
          return SyntaxKind.PlusEqualsToken;
        case SyntaxKind.SubtractAssignmentExpression:
          return SyntaxKind.MinusEqualsToken;
          
          */