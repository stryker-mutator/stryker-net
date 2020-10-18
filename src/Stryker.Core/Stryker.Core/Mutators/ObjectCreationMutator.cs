﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ObjectCreationMutator : MutatorBase<ObjectCreationExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ObjectCreationExpressionSyntax node)
        {
            if (node.Initializer?.Kind() == SyntaxKind.CollectionInitializerExpression && node.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.ReplaceNode(node.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression)),
                    DisplayName = "Collection initializer mutation",
                    Type = Mutator.Initializer
                };
            }
        }
    }
}
