using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.InjectedHelpers;

public class CodeInjection
{
    // files to be injected into the mutated assembly
    private static readonly string[] Files = {"Stryker.Core.InjectedHelpers.MutantControl.cs",
        "Stryker.Core.InjectedHelpers.Coverage.MutantContext.cs"};
    private const string PatternForCheck = "\\/\\/ *check with: *([^\\r\\n]+)";
    private const string MutantContextClassName = "MutantContext";
    private const string StrykerNamespace = "Stryker";
    private static readonly string Selector;

    static CodeInjection() //NOSONAR : no way to get read of static constructors
    {
        var helper = GetSourceFromResource("Stryker.Core.InjectedHelpers.MutantControl.cs");
        var extractor = new Regex(PatternForCheck);
        var result = extractor.Match(helper);
        if (!result.Success)
        {
            throw new InvalidDataException("Internal error: failed to find expression for mutant selection.");
        }

        Selector = result.Groups[1].Value;
    }

    public CodeInjection()
    {
        HelperNamespace = GetRandomNamespace();
        SelectorExpression = Selector.Replace(StrykerNamespace, HelperNamespace);

        foreach (var file in Files)
        {
            var fileContents = GetSourceFromResource(file).Replace(StrykerNamespace, HelperNamespace);
            MutantHelpers.Add(file, fileContents);
        }
    }

    public string SelectorExpression { get; }

    public string HelperNamespace { get; }

    private static string GetRandomNamespace()
    {
        // Create a string of characters and numbers allowed in the namespace  
        const string ValidChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();

        var chars = new char[15];
        for (var i = 0; i < 15; i++)
        {
            chars[i] = ValidChars[random.Next(0, ValidChars.Length)];
        }
        return StrykerNamespace + new string(chars);
    }

    public IDictionary<string, string> MutantHelpers { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Get a SyntaxNode describing the creation of static tracking object
    /// </summary>
    /// <returns>Returns new Stryker.MutantContext() with proper namespace</returns>
    public ObjectCreationExpressionSyntax GetContextClassConstructor() =>
        SyntaxFactory.ObjectCreationExpression(
            SyntaxFactory.QualifiedName(
                SyntaxFactory.IdentifierName(HelperNamespace),
                SyntaxFactory.IdentifierName(MutantContextClassName))
                .WithLeadingTrivia(SyntaxFactory.Space),
            SyntaxFactory.ArgumentList(),
            null);
    

    /// <summary>
    /// Get a SyntaxNode describing accessing a member of the mutant context class.
    /// </summary>
    /// <param name="member">Desired member</param>
    /// <returns>Returns Stryker.MutantContext.<paramref name="member"/> with proper namespace </returns>
    public  MemberAccessExpressionSyntax GetContextClassAccessExpression(string member) =>
        SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(HelperNamespace),
                SyntaxFactory.IdentifierName(MutantContextClassName)),
            SyntaxFactory.IdentifierName(member));


    /// <summary>
    /// Checks if an expression is describing accessing a member of the mutant context class.
    /// </summary>
    /// <param name="memberAccess">expression to analyze</param>
    /// <param name="member">expected member name</param>
    /// <returns>Returns true if the expression is Stryker.MutantContext.<paramref name="member"/> with proper namespace </returns>
    public static bool IsContextAccessExpression(ExpressionSyntax memberAccess, string member) =>
        memberAccess is MemberAccessExpressionSyntax {
            Expression: MemberAccessExpressionSyntax
            {
                Name: IdentifierNameSyntax { Identifier.ValueText: MutantContextClassName }
            },
            Name.Identifier.ValueText: { } specificMember
        } && specificMember == member;

    private static string GetSourceFromResource(string sourceResourceName)
    {
        using var stream = typeof(CodeInjection).Assembly.GetManifestResourceStream(sourceResourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
