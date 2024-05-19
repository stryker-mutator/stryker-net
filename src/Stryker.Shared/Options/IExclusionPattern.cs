using DotNet.Globbing;

namespace Stryker.Shared.Options;
public interface IExclusionPattern
{
    bool IsExcluded { get; }
    Glob Glob { get; }
    IEnumerable<(int Start, int End)> MutantSpans { get; }
}
