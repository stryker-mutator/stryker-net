using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Mutators;

public class StringMutator : MutatorBase<LiteralExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node, SemanticModel semanticModel)
    {
        // Get objectCreationSyntax to check if it contains a regex type.
        var root = node.Parent?.Parent?.Parent;

        if (IsSpecialType(root))
        {
            yield break;
        }

        SyntaxNode syntaxNode;
        string currentValue;
        string replacementValue;

        if (IsStringLiteral(node))
        {
            currentValue = (string)node.Token.Value;
            replacementValue = currentValue == "" ? "Stryker was here!" : "";
            syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(replacementValue));
        }
        else if(IsUtf8StringLiteral(node))
        {
            currentValue = (string)node.Token.Value;
            replacementValue = currentValue == "" ? "Stryker was here!" : "";
            syntaxNode = CreateUtf88String(node.GetLeadingTrivia(), replacementValue, node.GetTrailingTrivia());
        }
        else
        {
            yield break;
        }
            

        yield return new Mutation
        {
            OriginalNode = node,
            ReplacementNode = syntaxNode,
            DisplayName = "String mutation",
            Type = Mutator.String
        };
    }

    private static bool IsUtf8StringLiteral(LiteralExpressionSyntax node)
    {
        var kind = node.Kind();
        return kind is SyntaxKind.Utf8StringLiteralExpression
               && node.Parent is not ConstantPatternSyntax;
    }

    private static bool IsStringLiteral(LiteralExpressionSyntax node)
    {
        var kind = node.Kind();
        return kind is SyntaxKind.StringLiteralExpression
               && node.Parent is not ConstantPatternSyntax;
    }

    private static bool IsSpecialType(SyntaxNode root) => root switch
    {
        ObjectCreationExpressionSyntax ctor => IsCtorOfType(ctor, typeof(Regex)) || IsCtorOfType(ctor, typeof(Guid)),
        _ => false
    };

    private static bool IsCtorOfType(ObjectCreationExpressionSyntax ctor, Type type)
    {
        var ctorType = ctor.Type.ToString();
        return ctorType == type.Name || ctorType == type.FullName;
    }

    private static LiteralExpressionSyntax CreateUtf88String(SyntaxTriviaList leadingTrivia, string stringValue, SyntaxTriviaList trailingTrivia)
    {
        var quoteCharacter = '"';
        var suffix = "u8";

        var literal = SyntaxFactory.Token(
                leading: leadingTrivia,
                kind: SyntaxKind.Utf8StringLiteralToken,
                text: quoteCharacter + stringValue + quoteCharacter + suffix,
                valueText: "",
                trailing: trailingTrivia);

        return SyntaxFactory.LiteralExpression(SyntaxKind.Utf8StringLiteralExpression, literal);
    }

}
