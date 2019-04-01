﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Mutator Implementation for LINQ Mutations </summary>
    public class LinqMutator : MutatorBase<MemberAccessExpressionSyntax>, IMutator
    {
        /// <summary> Dictionary which maps original linq expressions to the target mutation </summary>
        private Dictionary<LinqExpression, LinqExpression> _kindsToMutate { get; }

        /// <summary> Constructor for the <see cref="LinqMutator"/> </summary>
        public LinqMutator()
        {
            _kindsToMutate = new Dictionary<LinqExpression, LinqExpression>
            {
                { LinqExpression.FirstOrDefault, LinqExpression.First },
                { LinqExpression.First, LinqExpression.FirstOrDefault },
                { LinqExpression.SingleOrDefault, LinqExpression.Single },
                { LinqExpression.Single, LinqExpression.SingleOrDefault },
                { LinqExpression.Last, LinqExpression.First },
                { LinqExpression.All, LinqExpression.Any },
                { LinqExpression.Any, LinqExpression.All },
                { LinqExpression.Skip, LinqExpression.Take },
                { LinqExpression.Take, LinqExpression.Skip },
                { LinqExpression.SkipWhile, LinqExpression.TakeWhile },
                { LinqExpression.TakeWhile, LinqExpression.SkipWhile },
                { LinqExpression.Min, LinqExpression.Max },
                { LinqExpression.Max, LinqExpression.Min },
                { LinqExpression.Sum, LinqExpression.Count },
                { LinqExpression.Count, LinqExpression.Sum },
                { LinqExpression.OrderBy, LinqExpression.OrderByDescending },
                { LinqExpression.OrderByDescending, LinqExpression.OrderBy },
                { LinqExpression.ThenBy, LinqExpression.ThenByDescending },
                { LinqExpression.ThenByDescending, LinqExpression.ThenBy }
            };
        }

        /// <summary> Apply mutations to an <see cref="MemberAccessExpressionSyntax"/> </summary>
        public override IEnumerable<Mutation> ApplyMutations(MemberAccessExpressionSyntax node)
        {
            if (Enum.TryParse(node.Name.Identifier.ValueText, out LinqExpression expression) &&
                _kindsToMutate.TryGetValue(expression, out LinqExpression replacementExpression) && node.Parent is InvocationExpressionSyntax)
            {
                SyntaxNode replacement = SyntaxFactory.IdentifierName(replacementExpression.ToString());
                string displayName = $"Linq method mutation ({node.Name.Identifier.ValueText}() to {replacement}())";

                if (replacementExpression.Equals(LinqExpression.None))
                {
                    replacement = SyntaxFactory.IdentifierName(string.Empty);
                    displayName = $"Linq method mutation (removed {node.Name.Identifier.ValueText})";
                }

                yield return new Mutation
                {
                    DisplayName = displayName,
                    OriginalNode = node,
                    ReplacementNode = node.ReplaceNode(node.Name, replacement),
                    Type = Mutator.Linq
                };
            }
        }
    }

    /// <summary> Enumeration for the different kinds of linq expressions </summary>
    public enum LinqExpression
    {
        None,
        Distinct,
        Reverse,
        OrderBy,
        OrderByDescending,
        FirstOrDefault,
        First,
        SingleOrDefault,
        Single,
        Last,
        All,
        Any,
        Skip,
        Take,
        SkipWhile,
        TakeWhile,
        Min,
        Max,
        Sum,
        Count,
        ThenBy,
        ThenByDescending
    }
}
