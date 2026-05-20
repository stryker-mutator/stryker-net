using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

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
        switch (node)
        {
            case MemberAccessExpressionSyntax memberAccessExpression:
                toReplace = memberAccessExpression.Name;
                memberName = memberAccessExpression.Name.Identifier.ValueText;
                break;
            case MemberBindingExpressionSyntax memberBindingExpression:
                toReplace = memberBindingExpression.Name;
                memberName = memberBindingExpression.Name.Identifier.ValueText;
                break;
            default:
                yield break;
        }

        if (Enum.TryParse(memberName, out LinqExpression expression) &&
            KindsToMutate.TryGetValue(expression, out var replacementExpression))
        {
            var invocation = FindInvocation(node);

            if (RequireArguments.Contains(replacementExpression) &&
                invocation?.ArgumentList.Arguments.Count == 0)
            {
                yield break;
            }

            if (!IsLinqInvocation(node, invocation, semanticModel))
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

    // `node` is the MemberAccess/MemberBinding being mutated. Its parent is the
    // enclosing InvocationExpression for a direct call (e.g. `x.Append(...)`);
    // for chained accesses we walk up through intermediate MemberAccess nodes.
    private static InvocationExpressionSyntax FindInvocation(ExpressionSyntax node)
    {
        if (node.Parent is InvocationExpressionSyntax direct)
        {
            return direct;
        }

        var current = node.Parent;
        while (current is MemberAccessExpressionSyntax or MemberBindingExpressionSyntax)
        {
            current = current.Parent;
            if (current is InvocationExpressionSyntax invocationExpression)
            {
                return invocationExpression;
            }
        }
        return null;
    }

    // Ensures we only mutate calls that are actually LINQ operations. Without this
    // check, any method whose name happens to match a LINQ operator (e.g.
    // IResponseCookies.Append) would be incorrectly rewritten.
    private static bool IsLinqInvocation(ExpressionSyntax node, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        if (semanticModel is null)
        {
            // No semantic model available (e.g. unit tests) — preserve legacy
            // name-only matching behaviour.
            return true;
        }

        if (invocation is null)
        {
            // Not an invocation (e.g. property/method-group reference like
            // `?.Count` or `var d = cookies.Append;`). Existing orchestrator
            // tests rely on the legacy name-only behaviour for these shapes, so
            // fall through and let Stryker's CompileError stage filter
            // uncompilable mutants.
            return true;
        }

        if (semanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol)
        {
            var containingType = methodSymbol.ContainingType?.ConstructedFrom ?? methodSymbol.ContainingType;
            if (containingType is not null && IsLinqHostType(containingType))
            {
                return true;
            }

            var receiverType = methodSymbol.IsExtensionMethod
                ? methodSymbol.ReducedFrom?.Parameters.FirstOrDefault()?.Type ?? methodSymbol.Parameters.FirstOrDefault()?.Type
                : methodSymbol.ReceiverType;

            return receiverType is not null && ImplementsGenericEnumerable(receiverType);
        }

        // Symbol couldn't be resolved from the invocation. Fall back to the
        // receiver's syntactic expression — if its type binds and it isn't a LINQ
        // shape, we can still skip the mutation. This catches cases like
        // `httpContext.Response.Cookies.Append(...)` where the broader semantic
        // resolution may have failed but the receiver's static type is still
        // available.
        if (node is MemberAccessExpressionSyntax memberAccess)
        {
            var receiverType = semanticModel.GetTypeInfo(memberAccess.Expression).Type;
            if (receiverType is not null && receiverType.TypeKind != TypeKind.Error)
            {
                return ImplementsGenericEnumerable(receiverType);
            }
        }

        // Receiver type could not be determined — preserve legacy behaviour.
        return true;
    }

    private static bool IsLinqHostType(INamedTypeSymbol type) =>
        type.ToDisplayString() is "System.Linq.Enumerable"
            or "System.Linq.Queryable"
            or "System.Linq.ParallelEnumerable";

    private static bool ImplementsGenericEnumerable(ITypeSymbol type)
    {
        if (IsGenericIEnumerable(type))
        {
            return true;
        }

        foreach (var iface in type.AllInterfaces)
        {
            if (IsGenericIEnumerable(iface))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsGenericIEnumerable(ITypeSymbol type) =>
        type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
}
