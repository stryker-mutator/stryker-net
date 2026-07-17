namespace Stryker.Core.DiffProviders;

/// <summary>
/// The kind of change a <see cref="DiffChange"/> represents, mirroring the operations produced by
/// the diff-match-patch algorithm.
/// </summary>
public enum DiffOperation
{
    Equal,
    Insert,
    Delete
}

/// <summary>
/// A single chunk of a text diff: either a piece of text that is unchanged between the old and
/// new source, or a piece of text that was inserted into the new source or deleted from the old source.
/// </summary>
public sealed class DiffChange
{
    public DiffOperation Operation { get; init; }
    public string Text { get; init; }
}
