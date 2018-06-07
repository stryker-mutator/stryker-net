using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace Stryker.Core.Compiling
{
    public interface IRollbackProcess
    {
        RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics);
    }
}