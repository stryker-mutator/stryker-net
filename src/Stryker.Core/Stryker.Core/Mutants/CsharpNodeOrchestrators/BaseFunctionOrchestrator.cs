using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.Instrumentation;


namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// This class implements a roslyn type independent orchestrator for method, functions, getters....
/// When adding a syntax construct, one needs to implement the various information extractor and operations to match Roslyn type
/// </summary>
/// <typeparam name="T">SyntaxNode type</typeparam>
/// <remarks>This class is helpful because there is no (useful) shared parent class for those syntax construct</remarks>
internal abstract class BaseFunctionOrchestrator<T> :MemberDefinitionOrchestrator<T>, IInstrumentCode where T : SyntaxNode
{
    protected BaseFunctionOrchestrator() => Marker = MutantPlacer.RegisterEngine(this, true);

    private SyntaxAnnotation Marker { get; }

    /// <inheritdoc/>
    public string InstrumentEngineId => GetType().Name;

    /// <summary>
    /// Get the function body (block or expression)
    /// </summary>
    /// <param name="node"></param>
    /// <returns>a tuple with the block body as first item and the expression body as the second. At least one of them is expected to be null.</returns>
    protected abstract (BlockSyntax block, ExpressionSyntax expression) GetBodies(T node);

    /// <summary>
    /// Gets the parameter list of the function
    /// </summary>
    /// <param name="node">instance of <see cref="T"/></param>
    /// <returns>a parameter list</returns>
    protected abstract ParameterListSyntax ParameterList(T node);

    /// <summary>
    /// Get the return type
    /// </summary>
    /// <param name="node">instance of <see cref="T"/></param>
    /// <returns>return type of the function</returns>
    protected abstract TypeSyntax ReturnType(T node);

    /// <summary>
    /// Use the provided syntax block for the body of the function (set the expression body part to null)
    /// </summary>
    /// <param name="node">instance of <see cref="T"/></param>
    /// <param name="blockBody">desired body</param>
    /// <param name="expressionBody">desired expression body</param>
    /// <returns>an instance of <typeparamref name="T"/> with <paramref name="blockBody"/> body</returns>
    protected abstract T SwitchToThisBodies(T node, BlockSyntax blockBody, ExpressionSyntax expressionBody);

    private static BlockSyntax GenerateBlockBody(ExpressionSyntax expressionBody, TypeSyntax returnType)
    {
        StatementSyntax statementLine = returnType.IsVoid()
            ? SyntaxFactory.ExpressionStatement(expressionBody)
            : SyntaxFactory.ReturnStatement(expressionBody.WithLeadingTrivia(SyntaxFactory.Space));

        var result = SyntaxFactory.Block(statementLine);
        return result;
    }

    public T ConvertToBlockBody(T node) => ConvertToBlockBody(node, ReturnType(node));

    protected T ConvertToBlockBody(T node, TypeSyntax returnType)
    {
        var (block, expression) = GetBodies(node);
        if (block != null)
        {
            return node;
        }
        var blockBody = GenerateBlockBody(expression, returnType);
        return SwitchToThisBodies(node, blockBody, null).WithAdditionalAnnotations(Marker);
    }

    /// <inheritdoc/>
    public SyntaxNode RemoveInstrumentation(SyntaxNode node)
    {
        if (node is not T typedNode)
        {
            throw new InvalidOperationException($"Expected a {typeof(T).ToString()}, found:\n{node.ToFullString()}.");
        }
        var (block, _) = GetBodies(typedNode);
        var expression = block?.Statements[0] switch
        {
            ReturnStatementSyntax returnStatement => returnStatement.Expression!,
            ExpressionStatementSyntax expressionStatement => expressionStatement.Expression!,
            _ => throw new InvalidOperationException($"Can't extract original expression from {block}")
        };

        return SwitchToThisBodies(typedNode, null, expression).WithoutAnnotations(Marker);
    }

    /// <inheritdoc/>
    protected override T InjectMutations(T sourceNode, T targetNode, SemanticModel semanticModel, MutationContext context)
    {
        var (blockBody, expressionBody) = GetBodies(targetNode);
        if (expressionBody == null && blockBody == null)
        {
            // no implementation provided
            return targetNode;
        }
        var wasInExpressionForm = GetBodies(sourceNode).expression != null;
        var returnType = ReturnType(sourceNode);
        var parameters = ParameterList(sourceNode).Parameters;

        // no mutations to inject
        if (!context.HasLeftOverMutations)
        {
            if (blockBody == null)
            {
                // we can't do any other injection
                return targetNode;
            }

            var originalBody = blockBody;
            // inject default initializers (if any)
            blockBody = MutantPlacer.InjectOutParametersInitialization(blockBody, parameters);
            if (!wasInExpressionForm)
            {
                // add ending return (to mitigate compilation error due to control flow change)
                // not needed for an expression form method as no control flow may be present
                blockBody = MutantPlacer.AddEndingReturn(blockBody, returnType);
            }
            // do we need to change the body
            return originalBody == blockBody ? targetNode : SwitchToThisBodies(targetNode, MutantPlacer.AddEndingReturn(blockBody, returnType), null);
        }

        targetNode = ConvertToBlockBody(targetNode, returnType);

        var newBody = MutantPlacer.InjectOutParametersInitialization(context.InjectMutations(GetBodies(targetNode).block, GetBodies(sourceNode).expression, !returnType.IsVoid()), parameters);
        targetNode = SwitchToThisBodies(targetNode, newBody, null);
        return targetNode;
    }

}
