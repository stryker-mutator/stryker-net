using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Stryker.Core.Compiling;


/// <summary>
/// Responsible for rolling back all mutations that prevent compiling the mutated assembly
/// </summary>
public class RollbackProcessResult
{
    public CSharpCompilation Compilation { get; set; }
    public IEnumerable<int> RollbackedIds { get; set; }
}
