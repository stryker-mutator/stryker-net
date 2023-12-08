using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public class PrefixUnaryMutator : MutatorBase<PrefixUnaryExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithOpposite = new Dictionary<SyntaxKind, SyntaxKind>
    {
        {SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression},
        {SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression},
        {SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression},
        {SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression},
    };

    private static readonly HashSet<SyntaxKind> UnaryToInitial = new HashSet<SyntaxKind>
    {
        SyntaxKind.BitwiseNotExpression,
        SyntaxKind.LogicalNotExpression
    };

    public override IEnumerable<Mutation> ApplyMutations(PrefixUnaryExpressionSyntax node, SemanticModel semanticModel)
    {
        var unaryKind = node.Kind();
        if (UnaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.PrefixUnaryExpression(oppositeKind, node.Operand),
                DisplayName = $"{unaryKind} to {oppositeKind} mutation",
                Type = unaryKind.ToString().StartsWith("Unary") ? Mutator.Unary : Mutator.Update
            };
        }
        else if (UnaryToInitial.Contains(unaryKind))
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = node.Operand,
                DisplayName = $"{unaryKind} to un-{unaryKind} mutation",
                Type = unaryKind.ToString().StartsWith("Logic") ? Mutator.Boolean : Mutator.Unary
            };
        }
    }
}
