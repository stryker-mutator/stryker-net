using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class InitializerMutator : MutatorBase<InitializerExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(InitializerExpressionSyntax node)
        {
            if (node.Parent is ArrayCreationExpressionSyntax || node.Parent is ImplicitArrayCreationExpressionSyntax || node.Parent is StackAllocArrayCreationExpressionSyntax || node.Expressions.Count <= 0)
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

            if (node.Kind() == SyntaxKind.ObjectInitializerExpression)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression),
                    DisplayName = "Object initializer mutation",
                    Type = Mutator.Initializer,
                };
            }
        }
    }
}
