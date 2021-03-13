using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Linq;

namespace Stryker.Core.Mutators
{
    public class StatementMutator: MutatorBase<StatementSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        private static readonly HashSet<SyntaxKind> ForbiddenSyntaxes = new HashSet<SyntaxKind>() {
            SyntaxKind.EmptyStatement,
            // removing variable declarations may result in a compilation error
            SyntaxKind.LocalDeclarationStatement
        };

        private static readonly HashSet<Type> ForbiddenBlockParentClasses = new HashSet<Type>() {
            typeof(BaseMethodDeclarationSyntax),
            typeof(AnonymousFunctionExpressionSyntax)
        };

        public override IEnumerable<Mutation> ApplyMutations(StatementSyntax node)
        {
            if (ForbiddenSyntaxes.Contains(node.Kind()))
            {
                yield break;
            }

            // non-void methods and lambdas blocks should not be removed
            if (node.Kind() == SyntaxKind.Block && ForbiddenBlockParentClasses.Any(t => t.IsAssignableFrom(node.Parent.GetType())))
            {
                yield break;
            }

            // non-void return statements may result in an error
            if (node is ReturnStatementSyntax returnNode && returnNode.Expression != null)
            {
                yield break;
            }

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.EmptyStatement(),
                DisplayName = @"Statement mutation",
                Type = Mutator.Statement
            };
        }
    }
}
