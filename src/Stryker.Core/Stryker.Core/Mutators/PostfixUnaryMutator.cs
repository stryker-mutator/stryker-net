﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutators
{
    public class PostfixUnaryMutator : Mutator<PostfixUnaryExpressionSyntax>, IMutator
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithOpposite = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression},
            {SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression},
        };

        public override IEnumerable<Mutation> ApplyMutations(PostfixUnaryExpressionSyntax node)
        {
            var unaryKind = node.Kind();
            if (UnaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.PostfixUnaryExpression(oppositeKind, node.Operand),
                    DisplayName = $"{unaryKind} to {oppositeKind} mutation",
                    Type = nameof(PostfixUnaryMutator)
                };
            }
        }
    }
}