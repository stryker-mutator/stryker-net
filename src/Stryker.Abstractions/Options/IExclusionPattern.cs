
using DotNet.Globbing;

namespace Stryker.Configuration.Options;

public interface IExclusionPattern
{
    Glob Glob { get; }
    bool IsExcluded { get; }
    IEnumerable<(int Start, int End)> MutantSpans { get; }
}
