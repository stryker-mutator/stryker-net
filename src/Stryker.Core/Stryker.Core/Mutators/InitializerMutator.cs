using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class InitializerMutator : MutatorBase<InitializerExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(InitializerExpressionSyntax node)
        {
            if (!(node.Parent is ArrayCreationExpressionSyntax) && !(node.Parent is ImplicitArrayCreationExpressionSyntax) && 
                node.Kind() == SyntaxKind.ArrayInitializerExpression && node.Expressions.Count > 0)
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
}
