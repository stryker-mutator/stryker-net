using Microsoft.CodeAnalysis;
using System;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Implements code instrumentation logic.
/// Note that instrumentation methods have specific signature and are not part of the interface.
/// </summary>
public interface IInstrumentCode
{
    /// <summary>
    /// Returns the specific identifier, used for roll back
    /// </summary>
    string InstrumentEngineId { get; }

    /// <summary>
    /// Removes instrumentation.
    /// </summary>
    /// <param name="node">Node to be cleared.</param>
    /// <returns>returns a node without the instrumentation.</returns>
    /// <exception cref="InvalidOperationException">if the node was not instrumented (by this instrumentingEngine)</exception>
    SyntaxNode RemoveInstrumentation(SyntaxNode node);
}
