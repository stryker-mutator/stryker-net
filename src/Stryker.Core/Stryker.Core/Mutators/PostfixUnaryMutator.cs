using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public class PostfixUnaryMutator : MutatorBase<PostfixUnaryExpressionSyntax>, IMutator
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithOpposite = new()
    {
        {SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression},
        {SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression},
    };

    public override IEnumerable<Mutation> ApplyMutations(PostfixUnaryExpressionSyntax node, SemanticModel semanticModel)
    {
        var unaryKind = node.Kind();
        if (UnaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.PostfixUnaryExpression(oppositeKind, node.Operand),
                DisplayName = $"{unaryKind} to {oppositeKind} mutation",
                Type = Mutator.Update
            };
        }
    }
}
