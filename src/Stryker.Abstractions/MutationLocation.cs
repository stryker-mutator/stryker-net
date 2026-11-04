using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions;

/// <summary>
/// Language-neutral source range of a mutation. Lines and columns are 1-based;
/// offsets are 0-based character offsets into the file text (end exclusive).
/// </summary>
public sealed record MutationLocation(
    string FilePath,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn,
    int StartOffset,
    int EndOffset)
{
    /// <summary>
    /// Converts a Roslyn span (0-based) to a 1-based location. This is the only place the offset is applied.
    /// </summary>
    public static MutationLocation FromRoslyn(FileLinePositionSpan span, int startOffset, int endOffset) => new(
        span.Path,
        span.StartLinePosition.Line + 1,
        span.StartLinePosition.Character + 1,
        span.EndLinePosition.Line + 1,
        span.EndLinePosition.Character + 1,
        startOffset,
        endOffset);

    public static MutationLocation FromRoslyn(SyntaxNode node) =>
        FromRoslyn(node.GetLocation().GetMappedLineSpan(), node.Span.Start, node.Span.End);
}
