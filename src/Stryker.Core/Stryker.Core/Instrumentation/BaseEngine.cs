using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Mutants;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Base logic for all instrumentation helpers. It takes of generating a specific annotation
/// </summary>
/// <typeparam name="T">SyntaxNode type handled by this helper.</typeparam>
/// <remarks>There is no standard helper injection method as each injector may require specific arguments.</remarks>
/// <remarks>Multiple helpers can work on the same type of constructs.</remarks>
internal abstract class BaseEngine<T>: IInstrumentCode where T: CSharpSyntaxNode
{
    protected BaseEngine() => Marker = MutantPlacer.RegisterEngine(this);

    /// <summary>
    /// Annotation to be added to the instrumented node
    /// </summary>
    protected SyntaxAnnotation Marker { get; }

    /// <summary>
    /// Engine name. Used by rollback logic to forward rollback to the proper engine.
    /// </summary>
    public string InstrumentEngineId => GetType().Name;

    /// <summary>
    /// Removes the instrumentation.
    /// </summary>
    /// <param name="node">node to be cleaned</param>
    /// <returns></returns>
    protected abstract SyntaxNode Revert(T node);

    /// <summary>
    /// Removes the helper from the code. Ensure that the node is of the proper type
    /// and forwards to specific implementation.
    /// </summary>
    /// <param name="node">Syntax node where the helper is located.</param>
    /// <returns>The non instrumented node.</returns>
    public SyntaxNode RemoveInstrumentation(SyntaxNode node)
    {
        if (node is T tNode)
        {
            return Revert(tNode);
        }
        throw new InvalidOperationException($"Expected a {typeof(T).Name}, found:\n{node.ToFullString()}.");
    }
}
