using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Compiling;

public record CSharpRollbackProcessResult(
    Compilation Compilation,
    IEnumerable<int> RollbackedIds);
