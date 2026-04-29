using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Compiling;

public record CSharpRollbackProcessResult(
    Compilation Compilation,
    IEnumerable<int> RollbackedIds);
