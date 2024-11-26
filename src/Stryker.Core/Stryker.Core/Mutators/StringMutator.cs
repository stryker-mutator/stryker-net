using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.RegexMutators;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Stryker.Core.Mutators;

public class StringMutator : IMutator
{
    private ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<StringMutator>();

    public MutationLevel RegexMutationLevel => MutationLevel.Advanced;

    public MutationLevel OtherMutationLevel => MutationLevel.Standard;

    public IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options)
    {
        if (node is LiteralExpressionSyntax tNode &&
            node.Kind() is SyntaxKind.StringLiteralExpression or SyntaxKind.Utf8StringLiteralExpression)
        {
            // the node was of the expected type, so invoke the mutation method
            return ApplyMutations(tNode, semanticModel, options.MutationLevel);
        }

        return [];
    }

    public IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node, SemanticModel semanticModel,
                                                MutationLevel           mutationLevel)
    {
        if (IsRegexString(node, semanticModel))
        {
            return ApplyRegexMutations(node, mutationLevel);
        }

        if (OtherMutationLevel <= mutationLevel && !IsGuidType(node.Parent?.Parent?.Parent, semanticModel) &&
            ApplyStringMutations(node) is { } mutation)
        {
            return [mutation];
        }

        return [];
    }

    private static Mutation ApplyStringMutations(LiteralExpressionSyntax node)
    {
        var currentValue = (string)node.Token.Value;
        var replacementValue = currentValue == "" ? "Stryker was here!" : "";


        LiteralExpressionSyntax replacement;
        if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            replacement = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(replacementValue));
        }
        else if (node.IsKind(SyntaxKind.Utf8StringLiteralExpression) && !InAdditionOperator(node))
        {
            replacement = CreateUtf88String(node.GetLeadingTrivia(), replacementValue, node.GetTrailingTrivia());
        }
        else
        {
            return null;
        }

        return new Mutation
        {
            OriginalNode    = node,
            ReplacementNode = replacement,
            DisplayName     = "String mutation",
            Type            = Mutator.String
        };
    }

    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/utf8-string-literals#addition-operator
    private static bool InAdditionOperator(LiteralExpressionSyntax node) =>
        node.AncestorsAndSelf()
         .Any(static a => a.IsKind(SyntaxKind.AddExpression) && a.DescendantNodes()
                                                              .OfType<LiteralExpressionSyntax>()
                                                              .All(static b => b.IsKind(SyntaxKind.Utf8StringLiteralExpression)));

    private IEnumerable<Mutation> ApplyRegexMutations(LiteralExpressionSyntax node, MutationLevel mutationLevel)
    {
        if (RegexMutationLevel > mutationLevel)
        {
            yield break;
        }

        var currentValue = node.Token.ValueText;
        var regexMutantOrchestrator = new RegexMutantOrchestrator(currentValue);
        var replacementValues = regexMutantOrchestrator.Mutate();

        foreach (var regexMutation in replacementValues)
        {
            try
            {
                _ = new Regex(regexMutation.ReplacementPattern);
            }
            catch (ArgumentException exception)
            {
                Logger?.LogDebug(
                                 "RegexMutator created mutation {CurrentValue} -> {ReplacementPattern} which is an invalid regular expression:\n{Message}",
                                 currentValue, regexMutation.ReplacementPattern, exception.Message);
                continue;
            }

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode =
                    LiteralExpression(SyntaxKind.StringLiteralExpression,
                                      Literal(regexMutation.ReplacementPattern)),
                DisplayName = regexMutation.DisplayName,
                Type        = Mutator.Regex,
                Description = regexMutation.Description
            };
        }
    }

    private static bool IsRegexString(SyntaxNode node, SemanticModel semanticModel)
    {
        foreach (var syntaxNode in node.AncestorsAndSelf())
        {
            switch (syntaxNode)
            {
                case ObjectCreationExpressionSyntax ctor:
                    return IsCtorOfType(ctor, typeof(Regex));
                case ImplicitObjectCreationExpressionSyntax ctor:
                    return IsCtorOfType(ctor, typeof(Regex), semanticModel);
                case ArgumentSyntax { Parent.Parent: InvocationExpressionSyntax parentInvocation } argument
                    when semanticModel?.GetOperation(parentInvocation) is IInvocationOperation invocationOp:
                    var argumentOp = invocationOp.Arguments.SingleOrDefault(a => a.Syntax == argument);
                    return argumentOp?.Parameter?.Type is INamedTypeSymbol
                               {
                                   TypeKind: TypeKind.Structure, IsRefLikeType: true, IsReadOnly: true,
                                   IsValueType: true, MetadataName: "ReadOnlySpan`1", CanBeReferencedByName: true,
                                   TypeArguments: [{ SpecialType: SpecialType.System_Char or SpecialType.System_Byte }]
                               } or
                               {
                                   SpecialType: SpecialType.System_String or SpecialType.System_Object
                               } &&
                           argumentOp.Parameter.GetAttributes().Any(IsRegexSyntaxAttribute);
                case FieldDeclarationSyntax field:
                    return field.AttributeLists.Any(static a => a.Attributes.Any(IsRegexSyntaxAttribute));
                case PropertyDeclarationSyntax field:
                    return field.AttributeLists.Any(static a => a.Attributes.Any(IsRegexSyntaxAttribute));
                case AssignmentExpressionSyntax assignment
                    when semanticModel?.GetOperation(assignment) is IAssignmentOperation
                    {
                        Type.SpecialType: SpecialType.System_String,
                        Target: IMemberReferenceOperation
                        {
                            Member: var member
                        }
                    }:
                    return member.GetAttributes().Any(IsRegexSyntaxAttribute);
                case BlockSyntax: // Early exits
                case MemberDeclarationSyntax:
                case UsingDirectiveSyntax:
                    return false;
            }
        }

        return false;
    }

    private static bool IsRegexSyntaxAttribute(AttributeSyntax attributeSyntax) =>
        IsStringSyntaxAttribute(attributeSyntax.Name)        &&
        attributeSyntax.ArgumentList?.Arguments is [var arg] &&
        (arg.Expression is
             LiteralExpressionSyntax { Token.ValueText: "Regex" } or
             IdentifierNameSyntax { Identifier.Text: "Regex" } ||
         (arg.Expression is MemberAccessExpressionSyntax
             {
                 Expression: var e,
                 Name: IdentifierNameSyntax { Identifier.Text: "Regex" }
             } && IsStringSyntaxAttribute(e)));

    private static bool IsStringSyntaxAttribute(ExpressionSyntax attributeSyntax) =>
        attributeSyntax is IdentifierNameSyntax
        {
            Identifier.Text: "StringSyntax" or "StringSyntaxAttribute"
        } or MemberAccessExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax
                    {
                        Identifier.Text: "System"
                    },
                    Name: IdentifierNameSyntax
                    {
                        Identifier.Text: "Diagnostics"
                    }
                },
                Name: IdentifierNameSyntax
                {
                    Identifier.Text: "CodeAnalysis"
                }
            },
            Name: IdentifierNameSyntax
            {
                Identifier.Text: "StringSyntax" or "StringSyntaxAttribute"
            }
        };

    private static bool IsStringSyntaxAttribute(NameSyntax attributeSyntax) =>
        attributeSyntax is IdentifierNameSyntax
        {
            Identifier.Text: "StringSyntax" or "StringSyntaxAttribute"
        } or QualifiedNameSyntax
        {
            Left: QualifiedNameSyntax
            {
                Left: QualifiedNameSyntax
                {
                    Left: IdentifierNameSyntax
                    {
                        Identifier.Text: "System"
                    },
                    Right: IdentifierNameSyntax
                    {
                        Identifier.Text: "Diagnostics"
                    }
                },
                Right: IdentifierNameSyntax
                {
                    Identifier.Text: "CodeAnalysis"
                }
            },
            Right: IdentifierNameSyntax
            {
                Identifier.Text: "StringSyntax" or "StringSyntaxAttribute"
            }
        };

    private static bool IsRegexSyntaxAttribute(AttributeData attributeData) =>
        (attributeData.AttributeClass?.Name.Equals("StringSyntaxAttribute") ?? false) &&
        attributeData.ConstructorArguments.FirstOrDefault().Value is StringSyntaxAttribute.Regex;

    private static bool IsGuidType(SyntaxNode root, SemanticModel semanticModel) => root switch
    {
        ObjectCreationExpressionSyntax ctor => IsCtorOfType(ctor, typeof(Guid)),
        ImplicitObjectCreationExpressionSyntax ctor => IsCtorOfType(ctor, typeof(Guid), semanticModel),
        _ => false
    };

    private static bool IsCtorOfType(ObjectCreationExpressionSyntax ctor, Type type)
    {
        var ctorType = ctor.Type.ToString();
        return ctorType == type.Name || ctorType == type.FullName;
    }

    private static bool IsCtorOfType(ImplicitObjectCreationExpressionSyntax ctor, Type type,
                                     SemanticModel                          semanticModel)
    {
        var ti = semanticModel?.GetTypeInfo(ctor);
        var ctorType = ti?.Type?.ToDisplayString();
        return ctorType == type.Name || ctorType == type.FullName;
    }

    private static LiteralExpressionSyntax CreateUtf88String(SyntaxTriviaList leadingTrivia, string stringValue,
                                                             SyntaxTriviaList trailingTrivia)
    {
        const char QuoteCharacter = '"';
        var literal = Token(leadingTrivia,
                            SyntaxKind.Utf8StringLiteralToken,
                            $"{QuoteCharacter}{stringValue}{QuoteCharacter}u8",
                            stringValue,
                            trailingTrivia);
        return LiteralExpression(SyntaxKind.Utf8StringLiteralExpression, literal);
    }
}
