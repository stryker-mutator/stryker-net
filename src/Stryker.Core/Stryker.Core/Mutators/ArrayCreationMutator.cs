using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ArrayCreationMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            if (node is ArrayCreationExpressionSyntax arrayCreationNode && arrayCreationNode.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation()
                {
                    OriginalNode = arrayCreationNode,
                    ReplacementNode = arrayCreationNode.ReplaceNode(arrayCreationNode.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            } else if (node is ImplicitArrayCreationExpressionSyntax implicitArrayCreationNode && implicitArrayCreationNode.Initializer.Expressions.Count > 0)
            {
                yield return new Mutation()
                {
                    OriginalNode = implicitArrayCreationNode,
                    ReplacementNode = implicitArrayCreationNode.ReplaceNode(implicitArrayCreationNode.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
            }
        }
    }
}
