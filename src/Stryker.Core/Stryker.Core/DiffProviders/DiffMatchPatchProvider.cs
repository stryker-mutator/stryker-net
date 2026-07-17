using System;
using System.Linq;
using DiffMatchPatch;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.DiffProviders;

/// <summary>
/// Computes a text-level diff between two arbitrary source strings using Google's diff-match-patch algorithm.
/// </summary>
public class DiffMatchPatchProvider : IDiffProvider
{
    public ITestSet Tests =>
        throw new NotSupportedException($"{nameof(DiffMatchPatchProvider)} does not track tests; it only supports content-level diffing.");

    public DiffResult ScanDiff() =>
        throw new NotSupportedException($"{nameof(DiffMatchPatchProvider)} does not support file-level diff scanning; use {nameof(GetContentDiff)} instead.");

    public DiffResult GetContentDiff(string oldSource, string newSource)
    {
        var differ = new diff_match_patch();
        var diffs = differ.diff_main(oldSource ?? string.Empty, newSource ?? string.Empty);

        var operations = diffs.Select(diff => new DiffChange
        {
            Operation = diff.operation switch
            {
                Operation.EQUAL => DiffOperation.Equal,
                Operation.INSERT => DiffOperation.Insert,
                Operation.DELETE => DiffOperation.Delete,
                _ => throw new NotSupportedException($"Unknown diff operation: {diff.operation}")
            },
            Text = diff.text
        }).ToList();

        return new DiffResult(operations, oldSource, newSource);
    }
}
