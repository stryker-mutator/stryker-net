using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary> Mutator Implementation for LINQ Mutations </summary>
    public class LinqMutator : MutatorBase<InvocationExpressionSyntax>, IMutator
    {
        /// <summary> Dictionary which maps original linq expressions to the target mutation </summary>
        private static Dictionary<LinqExpression, LinqExpression> KindsToMutate { get; }
       /// <summary> Dictionary which maps original linq expressions to the target mutation </summary>
        private static HashSet<LinqExpression> RequireArguments { get; }

        /// <summary> Constructor for the <see cref="LinqMutator"/> </summary>
        static LinqMutator()
        {
            KindsToMutate = new Dictionary<LinqExpression, LinqExpression>
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
            RequireArguments = new HashSet<LinqExpression>
            {
                LinqExpression.All,
                LinqExpression.SkipWhile,
                LinqExpression.TakeWhile,
                LinqExpression.Sum,
                LinqExpression.OrderBy,
                LinqExpression.OrderByDescending,
                LinqExpression.ThenBy,
                LinqExpression.ThenByDescending
            };
        }

        /// <summary> Apply mutations to an <see cref="InvocationExpressionSyntax"/> </summary>
        public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax parent)
        {
            if (parent.Expression is MemberAccessExpressionSyntax node)
            { 
                if (Enum.TryParse(node.Name.Identifier.ValueText, out LinqExpression expression) &&
                    KindsToMutate.TryGetValue(expression, out var replacementExpression))
                {
                    var replacement = SyntaxFactory.IdentifierName(replacementExpression.ToString());
                    var displayName = $"Linq method mutation ({node.Name.Identifier.ValueText}() to {replacement}())";

                    if (RequireArguments.Contains(replacementExpression) && parent.ArgumentList.Arguments.Count==0)
                    {
                        yield break;
                    }

                    yield return new Mutation
                    {
                        DisplayName = displayName,
                        OriginalNode = parent,
                        ReplacementNode = parent.ReplaceNode(node.Name, replacement),
                        Type = Mutator.Linq
                    };
                }
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
