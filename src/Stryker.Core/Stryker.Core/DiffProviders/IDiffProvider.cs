using Stryker.Abstractions.Testing;

namespace Stryker.Core.DiffProviders;

public interface IDiffProvider
{
    DiffResult ScanDiff();

    ITestSet Tests { get; }

    /// <summary>
    /// Computes a content-level diff between two versions of a file's source, used to remap
    /// mutant/test locations across runs (e.g. by the baseline feature). Every provider must
    /// explicitly state whether it supports this, either by implementing it or by throwing
    /// <see cref="System.NotSupportedException"/>.
    /// </summary>
    DiffResult GetContentDiff(string oldSource, string newSource);
}
