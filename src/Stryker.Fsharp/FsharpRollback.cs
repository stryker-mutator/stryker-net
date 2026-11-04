using FSharp.Compiler.Diagnostics;
using Stryker.Abstractions.Exceptions;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp;

/// <summary>A clean compilation, the mutants that had to be rolled back to get it, and the attempts it took.</summary>
internal sealed record FsharpRollbackResult(FsharpCompileResult Compiled, IReadOnlyCollection<int> RolledBackIds, int Attempts);

/// <summary>
/// Compile-error rollback, mirroring the C# escalation (<c>CsharpCompilingProcess.CompileAndFixUntilSuccess</c>):
/// Normal drops only the splices the diagnostics point at, Aggressive drops every splice of the files that
/// errored, LastChance leaves those files unmutated. Bounded by <see cref="MaxAttempts"/>.
/// </summary>
internal static class FsharpRollback
{
    public const int MaxAttempts = 50;

    private enum Mode { Normal, Aggressive, LastChance }

    /// <param name="leaves">Files carrying splices.</param>
    /// <param name="compile">Compiles the project with the given full-path to mutated-text overrides.</param>
    public static FsharpRollbackResult Run(IReadOnlyCollection<FsharpFileLeaf> leaves, string helperNamespace,
        Func<IReadOnlyDictionary<string, string>, FsharpCompileResult> compile, int maxAttempts = MaxAttempts)
    {
        var active = leaves.ToDictionary(l => l, l => l.Splices.ToList());
        var rolledBack = new List<int>();
        FsharpCompileResult? last = null;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var applied = active.Where(a => a.Value.Count > 0)
                .ToDictionary(a => a.Key, a => FsharpMutantOrchestrator.ApplySplicesTracked(a.Key.SourceCode, a.Value, helperNamespace));
            last = compile(applied.ToDictionary(a => a.Key.FullPath, a => a.Value.Text));
            if (last.Success)
            {
                return new FsharpRollbackResult(last, rolledBack, attempt);
            }

            var mode = attempt == maxAttempts ? Mode.LastChance : attempt >= maxAttempts - 3 ? Mode.Aggressive : Mode.Normal;
            foreach (var error in last.Diagnostics.Where(d => d.Severity.IsError))
            {
                var leaf = LeafOf(error, last, applied.Keys) ?? throw Fatal("An error was reported outside the mutated files", last);
                var placed = applied[leaf];
                var splice = mode == Mode.Normal ? EnclosingSplice(error, placed) : null;
                var dropped = splice == null
                    ? active[leaf].ToList()
                    : active[leaf].Where(s => s.MutantId == splice.MutantId).ToList();
                foreach (var drop in dropped)
                {
                    active[leaf].Remove(drop);
                    rolledBack.Add(drop.MutantId);
                }
            }

            if (!last.Diagnostics.Any(d => d.Severity.IsError))
            {
                throw Fatal("The compiler failed without reporting an error", last);
            }
        }

        throw Fatal($"The mutated F# project still does not compile after {maxAttempts} attempts", last!);
    }

    private static FsharpFileLeaf? LeafOf(FSharpDiagnostic error, FsharpCompileResult result, IEnumerable<FsharpFileLeaf> candidates)
    {
        // overridden sources compile from temp copies: diagnostics carry the copy's path
        var path = result.SourceMap.GetValueOrDefault(error.FileName, error.FileName);
        return candidates.FirstOrDefault(l => string.Equals(l.FullPath, path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>The splice whose final range in the mutated text contains the start of the diagnostic.</summary>
    private static PlacedSplice? EnclosingSplice(FSharpDiagnostic error, AppliedSplices applied)
    {
        var lineStarts = FsharpLocations.LineStarts(applied.Text);
        if (error.StartLine < 1 || error.StartLine > lineStarts.Count)
        {
            return null;
        }

        var offset = lineStarts[error.StartLine - 1] + error.StartColumn;
        return applied.Placed.FirstOrDefault(p => p.StartOffset <= offset && offset < p.EndOffset);
    }

    private static GeneralStrykerException Fatal(string reason, FsharpCompileResult result)
    {
        var errors = string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity.IsError).Select(d => d.ToString()));
        return new GeneralStrykerException($"{reason}:{Environment.NewLine}{errors}");
    }
}
