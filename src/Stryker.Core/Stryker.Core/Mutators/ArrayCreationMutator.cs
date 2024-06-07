using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public class ArrayCreationMutator : MutatorBase<ExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node, SemanticModel semanticModel)
    {
        switch (node)
        {
            case StackAllocArrayCreationExpressionSyntax { Initializer.Expressions.Count: > 0 } stackAllocArray:
                yield return new Mutation
                {
                    OriginalNode = stackAllocArray,
                    ReplacementNode = stackAllocArray.ReplaceNode(stackAllocArray.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
                break;
            case ArrayCreationExpressionSyntax { Initializer.Expressions.Count: > 0 } arrayCreationNode:
                yield return new Mutation
                {
                    OriginalNode = arrayCreationNode,
                    ReplacementNode = arrayCreationNode.ReplaceNode(arrayCreationNode.Initializer, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)),
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
                break;
        }
    }
}
