using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ObjectCreationMutator : MutatorBase<ObjectCreationExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
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
            if (node.Initializer?.Kind() == SyntaxKind.ObjectInitializerExpression && node.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.ReplaceNode(node.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression)),
                    DisplayName = "Object initializer mutation",
                    Type = Mutator.Initializer,
                };
            }
        }
    }
}
