using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutators;

/// <summary> Mutator Implementation for LINQ Mutations </summary>
public class LinqMutator : MutatorBase<ExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    /// <summary> Dictionary which maps original linq expressions to the target mutation </summary>
    private static Dictionary<LinqExpression, LinqExpression> KindsToMutate { get; }
    /// <summary> Dictionary which maps original linq expressions to the target mutation </summary>
    private static HashSet<LinqExpression> RequireArguments { get; }

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
            { LinqExpression.Sum, LinqExpression.Max },
            { LinqExpression.Count, LinqExpression.Sum },
            { LinqExpression.Average, LinqExpression.Min },
            { LinqExpression.OrderBy, LinqExpression.OrderByDescending },
            { LinqExpression.OrderByDescending, LinqExpression.OrderBy },
            { LinqExpression.ThenBy, LinqExpression.ThenByDescending },
            { LinqExpression.ThenByDescending, LinqExpression.ThenBy },
            { LinqExpression.Reverse, LinqExpression.AsEnumerable },
            { LinqExpression.AsEnumerable, LinqExpression.Reverse },
            { LinqExpression.Union, LinqExpression.Intersect },
            { LinqExpression.Intersect, LinqExpression.Union },
            { LinqExpression.Concat, LinqExpression.Except },
            { LinqExpression.Except, LinqExpression.Concat },
            { LinqExpression.MinBy, LinqExpression.MaxBy },
            { LinqExpression.MaxBy, LinqExpression.MinBy },
            { LinqExpression.SkipLast, LinqExpression.TakeLast },
            { LinqExpression.TakeLast, LinqExpression.SkipLast },
            { LinqExpression.Order, LinqExpression.OrderDescending },
            { LinqExpression.OrderDescending, LinqExpression.Order },
            { LinqExpression.UnionBy, LinqExpression.IntersectBy },
            { LinqExpression.IntersectBy, LinqExpression.UnionBy },
            { LinqExpression.Append, LinqExpression.Prepend },
            { LinqExpression.Prepend, LinqExpression.Append }
        };
        RequireArguments = new HashSet<LinqExpression>
        {
            LinqExpression.All,
            LinqExpression.SkipWhile,
            LinqExpression.TakeWhile,
            LinqExpression.OrderBy,
            LinqExpression.OrderByDescending,
            LinqExpression.ThenBy,
            LinqExpression.ThenByDescending,
            LinqExpression.Union,
            LinqExpression.Intersect,
            LinqExpression.SkipLast,
            LinqExpression.TakeLast,
            LinqExpression.MaxBy,
            LinqExpression.MinBy,
            LinqExpression.IntersectBy,
            LinqExpression.UnionBy,
            LinqExpression.Append,
            LinqExpression.Prepend
        };
    }
    /// <summary> Apply mutations to an <see cref="InvocationExpressionSyntax"/> </summary>
    public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node, SemanticModel semanticModel)
    {

        string memberName;
        SyntaxNode toReplace;
        InvocationExpressionSyntax invocationExpression;
        switch (node)
        {
            case MemberAccessExpressionSyntax {Parent: InvocationExpressionSyntax memberParent} memberAccessExpression:
                invocationExpression = memberParent;
                toReplace = memberAccessExpression.Name;
                memberName = memberAccessExpression.Name.Identifier.ValueText;
                break;
            case MemberAccessExpressionSyntax  //NOSONAR
                {Parent: ConditionalAccessExpressionSyntax { Parent: InvocationExpressionSyntax memberParent }} memberAccessExpression:
                invocationExpression = memberParent;
                toReplace = memberAccessExpression.Name;
                memberName = memberAccessExpression.Name.Identifier.ValueText;
                break;
            case MemberBindingExpressionSyntax
            {
                Parent : InvocationExpressionSyntax memberParent
            } memberBindingExpression:
                invocationExpression = memberParent;
                toReplace = memberBindingExpression.Name;
                memberName = memberBindingExpression.Name.Identifier.ValueText;
                break;
            default:
                yield break;
        }

        if (!Enum.TryParse(memberName, out LinqExpression expression) ||
            !KindsToMutate.TryGetValue(expression, out var replacementExpression))
        {
            yield break;
        }


        if (RequireArguments.Contains(replacementExpression) &&
            invocationExpression.ArgumentList.Arguments.Count == 0)
        {
            yield break;
        }

        yield return new Mutation
        {
            DisplayName =
                $"Linq method mutation ({memberName}() to {SyntaxFactory.IdentifierName(replacementExpression.ToString())}())",
            OriginalNode = node,
            ReplacementNode = node.ReplaceNode(toReplace,
                SyntaxFactory.IdentifierName(replacementExpression.ToString())).WithCleanTrivia(),
            Type = Mutator.Linq
        };
    }
}
