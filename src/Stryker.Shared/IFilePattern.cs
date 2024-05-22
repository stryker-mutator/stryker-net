using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;

namespace Stryker.Shared;
public interface IFilePattern : IEquatable<IFilePattern>
{
    /// <summary>
    /// Gets the <see cref="Glob"/> matching the file path.
    /// </summary>
    Glob Glob { get; }

    /// <summary>
    /// Gets whether the file and text spans should be in- or excluded.
    /// </summary>
    public bool IsExclude { get; }

    /// <summary>
    /// Gets the the text spans of the file this pattern matches.
    /// </summary>
    public IReadOnlyCollection<TextSpan> TextSpans { get; }

    /// <summary>
    /// Checks whether a given file path and span matches the current file pattern.
    /// </summary>
    /// <param name="filePath">The full file path.</param>
    /// <param name="textSpan">The span of the text to check.</param>
    /// <returns>True if the file and span matches the pattern.</returns>
    bool IsMatch(string filePath, TextSpan textSpan);
}
