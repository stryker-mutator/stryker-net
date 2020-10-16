﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class CheckedMutator : MutatorBase<CheckedExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(CheckedExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.CheckedExpression)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.Expression,
                    DisplayName = "Remove checked expression",
                    Type = Mutator.Checked
                };
            }
        }
    }
}
