using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

public class InitializerMutator : MutatorBase<InitializerExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(InitializerExpressionSyntax node, SemanticModel semanticModel)
    {
        if (node.Parent is ArrayCreationExpressionSyntax || node.Parent is ImplicitArrayCreationExpressionSyntax || node.Parent is StackAllocArrayCreationExpressionSyntax || !node.Expressions.Any())
        {
            yield break;
        }
          
        if (node.Kind() == SyntaxKind.ArrayInitializerExpression)
        {
            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression),
                DisplayName = "Array initializer mutation",
                Type = Mutator.Initializer
            };
        }
    }
}
