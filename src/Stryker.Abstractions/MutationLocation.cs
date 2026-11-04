using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions;

/// <summary>
/// Language-neutral source range of a mutation. Lines and columns are 1-based.
/// </summary>
public sealed record MutationLocation(string FilePath, int StartLine, int StartColumn, int EndLine, int EndColumn)
{
    /// <summary>
    /// Converts a Roslyn span (0-based) to a 1-based location. This is the only place the offset is applied.
    /// </summary>
    public static MutationLocation FromRoslyn(FileLinePositionSpan span) => new(
        span.Path,
        span.StartLinePosition.Line + 1,
        span.StartLinePosition.Character + 1,
        span.EndLinePosition.Line + 1,
        span.EndLinePosition.Character + 1);

    public static MutationLocation FromRoslyn(SyntaxNode node) => FromRoslyn(node.GetLocation().GetMappedLineSpan());
}
