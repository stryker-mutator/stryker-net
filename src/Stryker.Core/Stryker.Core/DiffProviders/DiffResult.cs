using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.DiffProviders;

/// <summary>
/// The result of a diff. Either a file-level diff (a set of changed file paths, produced by
/// <see cref="IDiffProvider.ScanDiff"/>) or a content-level diff (the operations produced by
/// diffing two versions of a single file's source, produced by <see cref="IDiffProvider.GetContentDiff"/>).
/// </summary>
/// <remarks>
/// <see cref="ChangedTestFiles"/> and <see cref="ChangedSourceFiles"/> are legacy, file-level-only
/// fields kept for the "since" feature. They are obsolete and will be removed once file-level
/// diffing is replaced by content-level diffing everywhere.
/// </remarks>
public class DiffResult
{
    [Obsolete("File-level diffing is being replaced by content-level diffing (see Operations/GetContentDiff). This will be removed in a future version.")]
    public ICollection<string> ChangedTestFiles { get; set; }

    [Obsolete("File-level diffing is being replaced by content-level diffing (see Operations/GetContentDiff). This will be removed in a future version.")]
    public ICollection<string> ChangedSourceFiles { get; set; }

    private readonly SourceText _oldText;
    private readonly SourceText _newText;

    public DiffResult() { }

    public DiffResult(IReadOnlyList<DiffChange> operations, string oldSource, string newSource)
    {
        Operations = operations;
        _oldText = SourceText.From(oldSource ?? string.Empty);
        _newText = SourceText.From(newSource ?? string.Empty);
    }

    public IReadOnlyList<DiffChange> Operations { get; init; }

    /// <summary>
    /// Maps <paramref name="oldLocation"/> (a location in the old source) to its equivalent location in
    /// the new source. Returns <see langword="false"/> if the location overlaps an inserted or deleted
    /// region, meaning the underlying code changed and the location cannot be safely reused.
    /// </summary>
    public bool TryMapLocation(ILocation oldLocation, out ILocation newLocation)
    {
        newLocation = null;

        var oldStart = ToOffset(_oldText, oldLocation.Start);
        var oldEnd = ToOffset(_oldText, oldLocation.End);

        var oldCursor = 0;
        var newCursor = 0;

        foreach (var op in Operations)
        {
            var length = op.Text?.Length ?? 0;

            switch (op.Operation)
            {
                case DiffOperation.Equal:
                    // The whole baseline range must fit inside a single unchanged chunk to be safely reused.
                    if (oldStart >= oldCursor && oldEnd <= oldCursor + length)
                    {
                        var newStart = newCursor + (oldStart - oldCursor);
                        var newEnd = newCursor + (oldEnd - oldCursor);
                        newLocation = new Location
                        {
                            Start = ToPosition(_newText, newStart),
                            End = ToPosition(_newText, newEnd)
                        };
                        return true;
                    }
                    oldCursor += length;
                    newCursor += length;
                    break;
                case DiffOperation.Delete:
                    oldCursor += length;
                    break;
                case DiffOperation.Insert:
                    newCursor += length;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op.Operation), op.Operation, "Unknown diff operation.");
            }
        }

        return false;
    }

    private static int ToOffset(SourceText text, IPosition position)
    {
        var lineIndex = position.Line - 1;
        if (lineIndex < 0 || lineIndex >= text.Lines.Count)
        {
            return text.Length;
        }

        var textLine = text.Lines[lineIndex];
        var columnIndex = Math.Max(position.Column - 1, 0);
        return Math.Min(textLine.Start + columnIndex, textLine.EndIncludingLineBreak);
    }

    private static Position ToPosition(SourceText text, int offset)
    {
        var clamped = Math.Clamp(offset, 0, text.Length);
        var linePosition = text.Lines.GetLinePosition(clamped);
        return new Position
        {
            Line = linePosition.Line + 1,
            Column = linePosition.Character + 1
        };
    }
}
