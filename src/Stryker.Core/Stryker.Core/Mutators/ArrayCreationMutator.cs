using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ArrayCreationMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            if (node is StackAllocArrayCreationExpressionSyntax stackAllocArray && stackAllocArray.Initializer?.Expressions != null && stackAllocArray.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation
                {
                    OriginalNode = stackAllocArray,
                    ReplacementNode = stackAllocArray.ReplaceNode(stackAllocArray.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            }

            if (node is ArrayCreationExpressionSyntax arrayCreationNode && arrayCreationNode.Initializer?.Expressions != null && arrayCreationNode.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation
                {
                    OriginalNode = arrayCreationNode,
                    ReplacementNode = arrayCreationNode.ReplaceNode(arrayCreationNode.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            }

            if (node is ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocArray && implicitStackAllocArray.Initializer?.Expressions != null && implicitStackAllocArray.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation
                {
                    OriginalNode = implicitStackAllocArray,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxFactory.Token(SyntaxKind.DefaultKeyword)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            }

            if (node is ImplicitArrayCreationExpressionSyntax implicitArrayCreationNode && implicitArrayCreationNode.Initializer?.Expressions != null && implicitArrayCreationNode.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation
                {
                    OriginalNode = implicitArrayCreationNode,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxFactory.Token(SyntaxKind.DefaultKeyword)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            }
        }
    }
}
