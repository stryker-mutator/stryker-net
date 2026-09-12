using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
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
            // Implicit arrays (e.g. `new[]{1, 2}`) carry no explicit element type, so the mutant is
            // built by resolving the type via the SemanticModel and *parsing* an explicit empty array.
            // Parsing the replacement (rather than composing it with the SyntaxFactory) is load-bearing:
            // a factory-composed ArrayCreationExpression node drives Roslyn's
            // Binder.ConvertAndBindArrayInitialization into an IndexOutOfRangeException during
            // target-type inference. A parsed node does not, in any position. Arrays whose element
            // type contains an anonymous type (directly, or nested in an array/tuple/generic/containing
            // type), and error types, are skipped: neither can be written explicitly in C#.
            case ImplicitArrayCreationExpressionSyntax { Initializer.Expressions.Count: > 0 } implicitArray
                when semanticModel != null
                    && semanticModel.GetTypeInfo(implicitArray).Type is IArrayTypeSymbol arrayType
                    && arrayType.ElementType.TypeKind != TypeKind.Error
                    && !ContainsAnonymousType(arrayType.ElementType):
                var elementTypeDisplay = arrayType.ElementType.ToMinimalDisplayString(semanticModel, implicitArray.SpanStart);
                var replacement = SyntaxFactory.ParseExpression($"new {elementTypeDisplay}[] {{}}")
                    .WithLeadingTrivia(implicitArray.GetLeadingTrivia())
                    .WithTrailingTrivia(implicitArray.GetTrailingTrivia());
                yield return new Mutation
                {
                    OriginalNode = implicitArray,
                    ReplacementNode = replacement,
                    DisplayName = "Array initializer mutation",
                    Type = Mutator.Initializer
                };
                break;
        }
    }

    /// <summary>
    /// True when <paramref name="type"/> is, or contains nested within (array element, tuple element,
    /// generic type argument, or a containing generic type), an anonymous type — which cannot be
    /// rendered as an explicit type name.
    /// </summary>
    private static bool ContainsAnonymousType(ITypeSymbol type)
    {
        if (type is null)
        {
            return false;
        }
        if (type.IsAnonymousType)
        {
            return true;
        }
        if (type is IArrayTypeSymbol array)
        {
            return ContainsAnonymousType(array.ElementType);
        }
        if (type is INamedTypeSymbol named)
        {
            foreach (var arg in named.TypeArguments)
            {
                if (ContainsAnonymousType(arg))
                {
                    return true;
                }
            }

            // A nested type can inherit a containing generic type closed over an anonymous type
            // (e.g. Outer&lt;anonymous&gt;.Inner); recurse the containing type so those are caught too.
            return ContainsAnonymousType(named.ContainingType);
        }
        return false;
    }
}
