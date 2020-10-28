using Microsoft.CodeAnalysis;
using System;

namespace Stryker.Core.Instrumentation
{
    internal abstract class BaseEngine<T> : IInstrumentCode where T : SyntaxNode
    {

        protected BaseEngine(string markerId, string engineId)
        {
            InstrumentEngineID = engineId;
            Marker = new SyntaxAnnotation(markerId, engineId);
        }

        protected SyntaxAnnotation Marker { get; }

        public string InstrumentEngineID { get; }

        protected abstract SyntaxNode Revert(T node);

        public SyntaxNode RemoveInstrumentation(SyntaxNode node)
        {
            if (node is T tNode)
            {
                return Revert(tNode);
            }
            throw new InvalidOperationException($"Expected a block containing a conditional expressionn, found:\n{node.ToFullString()}.");
        }
    }
}
