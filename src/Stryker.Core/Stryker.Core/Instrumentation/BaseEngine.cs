using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Instrumentation
{
    internal abstract class BaseEngine<T>: IInstrumentCode where T: SyntaxNode
    {
        protected BaseEngine(string markerId)
        {
            Marker = new SyntaxAnnotation(markerId, InstrumentEngineID);
        }

        protected SyntaxAnnotation Marker { get; }

        public string InstrumentEngineID => GetType().Name;

        protected abstract SyntaxNode Revert(T node);

        public SyntaxNode RemoveInstrumentation(SyntaxNode node)
        {
            if (node is T tNode)
            {
                return Revert(tNode);
            }
            throw new InvalidOperationException($"Expected a {typeof(T).Name}, found:\n{node.ToFullString()}.");
        }
    }
}
