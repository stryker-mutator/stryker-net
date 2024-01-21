using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.Instrumentation;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;


namespace Stryker.Core.Mutants;

/// <summary>
/// This class implements a roslyn type independent orchestrator for method, functions, getters....
/// When adding a syntax construct, one needs to implement the various information extractor and operations to match Roslyn type
/// </summary>
/// <typeparam name="T">SyntaxNode type</typeparam>
/// <remarks>This class is helpful because there is no (useful) shared parent class for those syntax construct</remarks>
internal abstract class BaseFunctionOrchestrator<T>:NodeSpecificOrchestrator<T,T>, IInstrumentCode where T : SyntaxNode
{
    protected BaseFunctionOrchestrator()
    {
        Marker = new SyntaxAnnotation(MutantPlacer.Injector, InstrumentEngineId);
        MutantPlacer.RegisterEngine(this, true);
    }

    private SyntaxAnnotation Marker { get; }

    /// <inheritdoc/>
    public string InstrumentEngineId => GetType().Name;

    /// <inheritdoc/>
    protected override MutationContext PrepareContext(T node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Member));

    /// <inheritdoc/>
    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());

    protected abstract (BlockSyntax block, ExpressionSyntax expression) GetBodies(T node);

    /// <summary>
    /// Gets the parameter list of the function
    /// </summary>
    /// <param name="node">instance of <see cref="T"/></param>
    /// <returns>a parameter list</returns>
    protected abstract ParameterListSyntax Parameters(T node);

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
    /// <returns>an instance of <typeparamref name="T"/> with <paramref name="blockBody"/> body</returns>
    protected abstract T SwitchToThisBodies(T node, BlockSyntax blockBody, ExpressionSyntax expressionBody);

    private BlockSyntax GenerateBlockBody(ExpressionSyntax expressionBody, TypeSyntax returnType)
    {
        StatementSyntax statementLine = returnType.IsVoid()
            ? SyntaxFactory.ExpressionStatement(expressionBody)
            : SyntaxFactory.ReturnStatement(expressionBody.WithLeadingTrivia(SyntaxFactory.Space));

        var result = SyntaxFactory.Block(statementLine);
        return result;
    }
    public T ConvertToBlockBody(T node) => ConvertToBlockBody(node, ReturnType(node));

    private T ConvertToBlockBody(T node, TypeSyntax returnType)
    {
        var (block, expression) = GetBodies(node);
        if (block!=null)
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
            return node;
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
        var returnType = ReturnType(sourceNode);
        var parameters = Parameters(sourceNode);
        if (expressionBody == null && blockBody == null)
        {
            // no implementation provided
            return targetNode;
        }

        var outParameters = parameters.Parameters.Where(p => p.Modifiers.Any( m=> m.IsKind(SyntaxKind.OutKeyword)));
        // no mutations to inject
        if (!context.HasLeftOverMutations)
        {
            if (blockBody == null) return targetNode;
            blockBody = MutantPlacer.AddDefaultInitializers(blockBody, outParameters);
            return SwitchToThisBodies(targetNode, MutantPlacer.AddEndingReturn(blockBody, returnType), null);
        }

        targetNode = ConvertToBlockBody(targetNode, returnType);

        var newBody = MutantPlacer.AddDefaultInitializers(context.InjectMutations(GetBodies(targetNode).block, GetBodies(sourceNode).expression, !returnType.IsVoid()), outParameters);
        targetNode = SwitchToThisBodies(targetNode, newBody, null);
        return targetNode;
    }

}
