using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

internal class RedirectMethodEngine : BaseEngine<MethodDeclarationSyntax>
{
    private const string RedirectHints = "RedirectHints";

    public ClassDeclarationSyntax InjectRedirect(ClassDeclarationSyntax originalClass,
        ExpressionSyntax condition,
        MethodDeclarationSyntax originalMethod,
        MethodDeclarationSyntax mutatedMethod)
    {
        if (!originalClass.Contains(originalMethod))
        {
            throw new ArgumentException($"Syntax tree does not contains {originalMethod.Identifier}.", nameof(originalMethod));
        }

        // this is to avoid name clashes when multiple mutations are applied to the same method
        var index = 0;
        var newNameForOriginal = FindNewName(originalClass, originalMethod, ref index);
        var newNameForMutated = FindNewName(originalClass, originalMethod, ref index);

        // generates a redirecting method
        // generate calls to the redirected method
        var originalCall = GenerateRedirectedInvocation(originalMethod, newNameForOriginal);
        var mutatedCall = GenerateRedirectedInvocation(originalMethod, newNameForMutated);

        var redirectHints = new SyntaxAnnotation(RedirectHints, $"{originalMethod.Identifier},{newNameForOriginal},{newNameForMutated}");

        var redirector = originalMethod
            .WithBody(SyntaxFactory.Block(
                SyntaxFactory.IfStatement(condition, mutatedCall.AsBlock(),
                SyntaxFactory.ElseClause(originalCall.AsBlock())
                ))).WithExpressionBody(null).WithoutLeadingTrivia();

        // update the class
        var resultingClass = originalClass.RemoveNode(originalMethod, SyntaxRemoveOptions.KeepNoTrivia)
            ?.AddMembers(redirector.WithTrailingNewLine().WithAdditionalAnnotations(redirectHints, Marker),
                originalMethod.WithIdentifier(SyntaxFactory.Identifier(newNameForOriginal)).WithTrailingNewLine().WithAdditionalAnnotations(redirectHints, Marker),
                mutatedMethod.WithIdentifier(SyntaxFactory.Identifier(newNameForMutated)).WithTrailingNewLine().WithAdditionalAnnotations(redirectHints, Marker));
        return resultingClass;
    }

    private static InvocationExpressionSyntax GenerateRedirectedInvocation(MethodDeclarationSyntax originalMethod, string redirectedName)
        => SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(redirectedName),
            SyntaxFactory.ArgumentList( SyntaxFactory.SeparatedList(
                originalMethod.ParameterList.Parameters.Select(p => SyntaxFactory.Argument( SyntaxFactory.IdentifierName(p.Identifier))))));

    /// <summary>
    /// Finds a new name for the redirected method that does not clash with existing methods
    /// </summary>
    /// <param name="originalClass">class containing the method</param>
    /// <param name="originalMethod">method to find a name for</param>
    /// <param name="index">index used for naming, contains the next available value on return</param>
    /// <returns>a new class name that does not conflict with existing method names</returns>
    /// <remarks>Construct names in the 'currentName_index' form and increment the index if there is a collision.
    /// returned index value must be reused when generating multiple names to prevent any collision when injecting methods.</remarks>
    private static string FindNewName(ClassDeclarationSyntax originalClass, MethodDeclarationSyntax originalMethod, ref int index)
    {
        string newNameForOriginal;
        do
        {
            newNameForOriginal = $"{originalMethod.Identifier}_{index++}";
        }
        while (originalClass.Members.Any(m => m is MethodDeclarationSyntax method && method.Identifier.ToFullString() == newNameForOriginal));
        return newNameForOriginal;
    }

    protected override SyntaxNode Revert(MethodDeclarationSyntax node) => throw new NotSupportedException("Cannot revert node in place.");

    public override SyntaxNode RemoveInstrumentationFrom(SyntaxNode _, SyntaxNode instrumentation)
    {
        var annotation = instrumentation.GetAnnotations(RedirectHints).FirstOrDefault()?.Data;
        if (string.IsNullOrEmpty(annotation))
        {
            throw new InvalidOperationException($"Unable to find details to rollback this instrumentation: '{instrumentation}'");
        }

        var method = (MethodDeclarationSyntax) instrumentation;
        var names = annotation.Split(',').ToList();

        var parentClass = (ClassDeclarationSyntax) method.Parent;
        var renamedMethod = (MethodDeclarationSyntax) parentClass.Members.
            First( m=> m is MethodDeclarationSyntax meth && meth.Identifier.Text == names[1]);
        parentClass = parentClass.TrackNodes(renamedMethod);
        // we need to remove redirection method and replacement method and restore the name of the original method
        parentClass = parentClass.RemoveNamedMember(names[2]).RemoveNamedMember(names[0]);
        var oldNode = parentClass.GetCurrentNode(renamedMethod);
        parentClass = parentClass.ReplaceNode(oldNode, renamedMethod.WithIdentifier(SyntaxFactory.Identifier(names[0])));
        return parentClass;
    }
}
