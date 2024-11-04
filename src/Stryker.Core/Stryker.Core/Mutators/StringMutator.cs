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

namespace Stryker.Core.Mutators;

public class StringMutator : IMutator
{
    private ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<StringMutator>();

    public MutationLevel RegexMutationLevel => MutationLevel.Advanced;

    public MutationLevel OtherMutationLevel => MutationLevel.Standard;

    public IEnumerable<Mutation> Mutate(SyntaxNode node, SemanticModel semanticModel, IStrykerOptions options)
    {
        if (node is LiteralExpressionSyntax tNode && node.IsKind(SyntaxKind.StringLiteralExpression))
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
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                      SyntaxFactory.Literal(regexMutation
                                                                      .ReplacementPattern)),
                    DisplayName = regexMutation.DisplayName,
                    Type        = Mutator.Regex,
                    Description = regexMutation.Description
                };
            }
        }
        else if (OtherMutationLevel <= mutationLevel && !IsSpecialType(node.Parent?.Parent?.Parent))
        {
            var currentValue = (string)node.Token.Value;
            var replacementValue = currentValue == "" ? "Stryker was here!" : "";

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode =
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                    SyntaxFactory.Literal(replacementValue)),
                DisplayName = "String mutation",
                Type        = Mutator.String
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
                case ArgumentSyntax argument when semanticModel is not null &&
                                                  argument.Parent?.Parent is InvocationExpressionSyntax parentInvocation
                    :
                    var invocationOp = semanticModel?.GetOperation(parentInvocation) as IInvocationOperation;
                    var argumentOp = invocationOp?.Arguments.SingleOrDefault(a => a.Syntax == argument);

                    return argumentOp?.Parameter?.Type.Name is "String" &&
                           argumentOp.Parameter.GetAttributes().Any(IsRegexSyntaxAttribute);
                case FieldDeclarationSyntax field:
                    return field.AttributeLists.Any(static a => a.Attributes.Any(IsRegexSyntaxAttribute));
                case PropertyDeclarationSyntax field:
                    return field.AttributeLists.Any(static a => a.Attributes.Any(IsRegexSyntaxAttribute));
                case AssignmentExpressionSyntax assignment
                    when semanticModel?.GetOperation(assignment) is ISimpleAssignmentOperation
                    {
                        Type.SpecialType: SpecialType.System_String
                    } expressionOp:
                    return expressionOp?.Target switch
                    {
                        IFieldReferenceOperation field => field.Field.GetAttributes().Any(IsRegexSyntaxAttribute),
                        IPropertyReferenceOperation property => property.Property.GetAttributes()
                                                                     .Any(IsRegexSyntaxAttribute),
                        _ => false
                    };
            }
        }

        return false;
    }

    private static bool IsRegexSyntaxAttribute(AttributeSyntax attributeSyntax) =>
        IsStringSyntaxAttribute(attributeSyntax.Name)        &&
        attributeSyntax.ArgumentList?.Arguments is [var arg] &&
        (arg.Expression is
             LiteralExpressionSyntax { Token.Text: "Regex" } or
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

    private static bool IsSpecialType(SyntaxNode root) => root switch
    {
        ObjectCreationExpressionSyntax ctor => IsCtorOfType(ctor, typeof(Guid)),
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
}
