using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Injects initialization to default value for a parameter or a variable at the beginning of a method.
/// <remarks>No check is made on the visibility of the variable.</remarks>
/// <exception cref="InvalidOperationException">If the method has no body (virtual or expression form).</exception>
/// </summary>
/// <remarks>Parameters/variables are added on a one by one basis, but they are all removed simultaneously.</remarks>
internal class DefaultInitializationEngine : BaseEngine<BlockSyntax>
{
    private static readonly SyntaxAnnotation BlockMarker = new("InitializationBlock");

    /// <summary>
    /// Add an assignment to default value for the given parameter/variable
    /// </summary>
    /// <param name="body">method/accessor body where to inject the assignment</param>
    /// <param name="parameters">parameters to initialize</param>
    /// <returns>the method with a block containing initializations to default for the given variables/parameters</returns>
    public BlockSyntax InjectOutParametersInitialization(BlockSyntax body, IEnumerable<ParameterSyntax> parameters)
    {
        if (body == null)
        {
            throw new InvalidOperationException(
                "Can't add default initializer(s) to expression bodied or virtual method.");
        }
        parameters = parameters.Where(p => p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword))).ToList();
        if (!parameters.Any())
        {
            return body;
        }

        IEnumerable<StatementSyntax> initializers;
        IEnumerable<StatementSyntax> originalStatements;
        if (body.Statements.Count > 0 && body.Statements[0].HasAnnotation(BlockMarker))
        {
            // we add a new initializer, we need to get the existing ones
            initializers = ((BlockSyntax)body.Statements[0]).Statements;
            // we can skip the initializer helper block
            originalStatements = body.Statements.Skip(1);
        }
        else
        {
            // this is the first initializer helper, no pre existing ones
            initializers = Array.Empty<StatementSyntax>();
            // keep all statements
            originalStatements = body.Statements;
        }

        var initializersBlock = SyntaxFactory.Block(initializers.Union( parameters.Select( p => SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(p.Identifier),
                p.Type.BuildDefaultExpression())))))
            .WithAdditionalAnnotations(BlockMarker);

        return body.WithStatements(new(originalStatements.Prepend(initializersBlock))).WithAdditionalAnnotations(Marker);
    }

    /// <summary>
    /// Removes all initializer from the given method.
    /// </summary>
    /// <param name="body"></param>
    /// <returns>the method with the initialization block removed.</returns>
    protected override SyntaxNode Revert(BlockSyntax body)
    {
        if (body == null || body.Statements.Count == 0 || body.Statements[0].Kind() != SyntaxKind.Block)
        {
            throw new InvalidOperationException(
                "Can't find initializer block at the beginning of method.");
        }

        return body.WithStatements(new SyntaxList<StatementSyntax>(body.Statements.Skip(1)))
            .WithoutAnnotations(Marker);
    }
}
