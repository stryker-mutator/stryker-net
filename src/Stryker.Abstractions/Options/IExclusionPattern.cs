
using DotNet.Globbing;

namespace Stryker.Abstractions.Options;

public interface IExclusionPattern
{
    Glob Glob { get; }
    bool IsExcluded { get; }
    IEnumerable<(int Start, int End)> MutantSpans { get; }
}
